using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 事件订阅管理器，防止内存泄漏并提供自动清理功能
/// </summary>
public class EventSubscriptionManager : IDisposable
{
    private static readonly Lazy<EventSubscriptionManager> _instance = new(() => new EventSubscriptionManager());
    public static EventSubscriptionManager Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, List<WeakEventSubscription>> _subscriptions = new();
    private readonly Timer _cleanupTimer;
    private readonly object _lock = new();
    private bool _disposed = false;

    private const int CleanupIntervalMs = 60000; // 1分钟清理间隔

    private EventSubscriptionManager()
    {
        _cleanupTimer = new Timer(CleanupDeadReferences, null, CleanupIntervalMs, CleanupIntervalMs);
    }

    /// <summary>
    /// 订阅事件，返回可用于取消订阅的令牌
    /// </summary>
    public IDisposable Subscribe<T>(string eventKey, T target, Action<T> handler) where T : class
    {
        if (_disposed) throw new ObjectDisposedException(nameof(EventSubscriptionManager));
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var subscription = new WeakEventSubscription<T>(target, handler);
        
        lock (_lock)
        {
            if (!_subscriptions.TryGetValue(eventKey, out var list))
            {
                list = new List<WeakEventSubscription>();
                _subscriptions[eventKey] = list;
            }
            list.Add(subscription);
        }

        return new SubscriptionToken(() => Unsubscribe(eventKey, subscription));
    }

    /// <summary>
    /// 订阅UI事件，自动在UI线程上执行
    /// </summary>
    public IDisposable SubscribeUI<T>(string eventKey, T target, Action<T> handler) where T : class
    {
        return Subscribe(eventKey, target, t =>
        {
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                handler(t);
            }
            else
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => handler(t));
            }
        });
    }

    /// <summary>
    /// 发布事件到所有订阅者
    /// </summary>
    public void Publish<T>(string eventKey, T eventData)
    {
        if (_disposed) return;

        List<WeakEventSubscription> subscriptions = null;
        
        lock (_lock)
        {
            if (_subscriptions.TryGetValue(eventKey, out var list))
            {
                subscriptions = new List<WeakEventSubscription>(list);
            }
        }

        if (subscriptions == null) return;

        var deadSubscriptions = new List<WeakEventSubscription>();

        foreach (var subscription in subscriptions)
        {
            if (subscription.IsAlive)
            {
                try
                {
                    subscription.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"事件处理器执行失败: {ex.Message}");
                }
            }
            else
            {
                deadSubscriptions.Add(subscription);
            }
        }

        // 清理死亡的订阅
        if (deadSubscriptions.Count > 0)
        {
            lock (_lock)
            {
                if (_subscriptions.TryGetValue(eventKey, out var list))
                {
                    foreach (var dead in deadSubscriptions)
                    {
                        list.Remove(dead);
                    }
                    
                    if (list.Count == 0)
                    {
                        _subscriptions.TryRemove(eventKey, out _);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 异步发布事件
    /// </summary>
    public Task PublishAsync<T>(string eventKey, T eventData)
    {
        return Task.Run(() => Publish(eventKey, eventData));
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    private void Unsubscribe(string eventKey, WeakEventSubscription subscription)
    {
        if (_disposed) return;

        lock (_lock)
        {
            if (_subscriptions.TryGetValue(eventKey, out var list))
            {
                list.Remove(subscription);
                if (list.Count == 0)
                {
                    _subscriptions.TryRemove(eventKey, out _);
                }
            }
        }
    }

    /// <summary>
    /// 清理所有指定事件的订阅
    /// </summary>
    public void ClearSubscriptions(string eventKey)
    {
        if (_disposed) return;

        lock (_lock)
        {
            _subscriptions.TryRemove(eventKey, out _);
        }
    }

    /// <summary>
    /// 清理所有订阅
    /// </summary>
    public void ClearAllSubscriptions()
    {
        if (_disposed) return;

        lock (_lock)
        {
            _subscriptions.Clear();
        }
    }

    /// <summary>
    /// 获取订阅统计信息
    /// </summary>
    public SubscriptionStats GetStats()
    {
        lock (_lock)
        {
            var totalSubscriptions = _subscriptions.Values.Sum(list => list.Count);
            var aliveSubscriptions = _subscriptions.Values.Sum(list => list.Count(s => s.IsAlive));
            
            return new SubscriptionStats
            {
                EventCount = _subscriptions.Count,
                TotalSubscriptions = totalSubscriptions,
                AliveSubscriptions = aliveSubscriptions,
                DeadSubscriptions = totalSubscriptions - aliveSubscriptions
            };
        }
    }

    private void CleanupDeadReferences(object state)
    {
        if (_disposed) return;

        var keysToRemove = new List<string>();
        
        lock (_lock)
        {
            foreach (var kvp in _subscriptions.ToList())
            {
                var deadSubscriptions = kvp.Value.Where(s => !s.IsAlive).ToList();
                
                foreach (var dead in deadSubscriptions)
                {
                    kvp.Value.Remove(dead);
                }
                
                if (kvp.Value.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                _subscriptions.TryRemove(key, out _);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _cleanupTimer?.Dispose();
        ClearAllSubscriptions();
    }

    // 弱引用事件订阅基类
    private abstract class WeakEventSubscription
    {
        public abstract bool IsAlive { get; }
        public abstract void Invoke(object eventData);
    }

    // 泛型弱引用事件订阅
    private class WeakEventSubscription<T> : WeakEventSubscription where T : class
    {
        private readonly WeakReference<T> _targetRef;
        private readonly Action<T> _handler;

        public WeakEventSubscription(T target, Action<T> handler)
        {
            _targetRef = new WeakReference<T>(target);
            _handler = handler;
        }

        public override bool IsAlive => _targetRef.TryGetTarget(out _);

        public override void Invoke(object eventData)
        {
            if (_targetRef.TryGetTarget(out var target))
            {
                _handler(target);
            }
        }
    }

    // 订阅令牌
    private class SubscriptionToken : IDisposable
    {
        private readonly Action _unsubscribe;
        private bool _disposed = false;

        public SubscriptionToken(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _unsubscribe?.Invoke();
            }
        }
    }

    public class SubscriptionStats
    {
        public int EventCount { get; set; }
        public int TotalSubscriptions { get; set; }
        public int AliveSubscriptions { get; set; }
        public int DeadSubscriptions { get; set; }
        
        public double DeadSubscriptionPercentage => TotalSubscriptions > 0 ? (double)DeadSubscriptions / TotalSubscriptions * 100 : 0;
    }
}
