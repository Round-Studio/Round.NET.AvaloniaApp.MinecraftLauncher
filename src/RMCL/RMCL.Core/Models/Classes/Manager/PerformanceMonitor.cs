using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 性能监控管理器，用于监控内存使用、CPU使用率等性能指标
/// </summary>
public class PerformanceMonitor : IDisposable
{
    private static readonly Lazy<PerformanceMonitor> _instance = new(() => new PerformanceMonitor());
    public static PerformanceMonitor Instance => _instance.Value;

    private readonly Timer _monitorTimer;
    private readonly Process _currentProcess;
    private PerformanceCounter _cpuCounter;
    private bool _disposed = false;
    private bool _cpuCounterAvailable = false;

    // 性能数据
    private long _lastGcMemory = 0;
    private DateTime _lastGcTime = DateTime.UtcNow;
    private int _gcCollectionCount0 = 0;
    private int _gcCollectionCount1 = 0;
    private int _gcCollectionCount2 = 0;

    // 配置参数
    private const int MonitorIntervalMs = 5000; // 5秒监控间隔
    private const long MemoryWarningThreshold = 500 * 1024 * 1024; // 500MB警告阈值
    private const double CpuWarningThreshold = 80.0; // 80% CPU警告阈值

    public event Action<PerformanceData> PerformanceDataUpdated;
    public event Action<string> PerformanceWarning;

    private PerformanceMonitor()
    {
        _currentProcess = Process.GetCurrentProcess();

        // 尝试初始化CPU计数器，如果失败则跳过
        try
        {
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _currentProcess.ProcessName);
            _cpuCounterAvailable = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CPU性能计数器初始化失败: {ex.Message}");
            _cpuCounterAvailable = false;
        }

        // 初始化GC计数
        _gcCollectionCount0 = GC.CollectionCount(0);
        _gcCollectionCount1 = GC.CollectionCount(1);
        _gcCollectionCount2 = GC.CollectionCount(2);

        _monitorTimer = new Timer(MonitorPerformance, null, MonitorIntervalMs, MonitorIntervalMs);
    }

    private void MonitorPerformance(object state)
    {
        if (_disposed) return;

        try
        {
            var data = CollectPerformanceData();
            PerformanceDataUpdated?.Invoke(data);
            
            CheckPerformanceWarnings(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"性能监控出错: {ex.Message}");
        }
    }

    private PerformanceData CollectPerformanceData()
    {
        // 内存信息
        var gcMemory = GC.GetTotalMemory(false);
        var workingSet = _currentProcess.WorkingSet64;
        var privateMemory = _currentProcess.PrivateMemorySize64;

        // CPU信息
        var cpuUsage = 0f;
        if (_cpuCounterAvailable)
        {
            try
            {
                cpuUsage = _cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CPU使用率获取失败: {ex.Message}");
                _cpuCounterAvailable = false;
            }
        }

        // GC信息
        var currentGc0 = GC.CollectionCount(0);
        var currentGc1 = GC.CollectionCount(1);
        var currentGc2 = GC.CollectionCount(2);

        var gcCollections0 = currentGc0 - _gcCollectionCount0;
        var gcCollections1 = currentGc1 - _gcCollectionCount1;
        var gcCollections2 = currentGc2 - _gcCollectionCount2;

        _gcCollectionCount0 = currentGc0;
        _gcCollectionCount1 = currentGc1;
        _gcCollectionCount2 = currentGc2;

        // 线程信息
        var threadCount = _currentProcess.Threads.Count;

        // 缓存信息
        var imageCacheStats = ImageCacheManager.Instance.GetStats();
        var eventStats = EventSubscriptionManager.Instance.GetStats();

        return new PerformanceData
        {
            Timestamp = DateTime.UtcNow,
            
            // 内存
            GcMemory = gcMemory,
            WorkingSet = workingSet,
            PrivateMemory = privateMemory,
            
            // CPU
            CpuUsage = cpuUsage,
            
            // GC
            GcCollections0 = gcCollections0,
            GcCollections1 = gcCollections1,
            GcCollections2 = gcCollections2,
            
            // 线程
            ThreadCount = threadCount,
            
            // 缓存
            ImageCacheItemCount = imageCacheStats.ItemCount,
            ImageCacheMemoryUsage = imageCacheStats.MemoryUsage,
            ImageCacheMemoryUsagePercentage = imageCacheStats.MemoryUsagePercentage,

            // 事件
            EventSubscriptionCount = eventStats.EventCount,
            AliveSubscriptions = eventStats.AliveSubscriptions,
            DeadSubscriptions = eventStats.DeadSubscriptions
        };
    }

    private void CheckPerformanceWarnings(PerformanceData data)
    {
        // 内存警告
        if (data.WorkingSet > MemoryWarningThreshold)
        {
            PerformanceWarning?.Invoke($"内存使用过高: {data.WorkingSet / 1024 / 1024}MB");
        }

        // CPU警告
        if (data.CpuUsage > CpuWarningThreshold)
        {
            PerformanceWarning?.Invoke($"CPU使用率过高: {data.CpuUsage:F1}%");
        }

        // GC频繁警告
        if (data.GcCollections2 > 0)
        {
            PerformanceWarning?.Invoke($"Gen2 GC发生: {data.GcCollections2}次");
        }

        // 死亡订阅警告
        if (data.DeadSubscriptions > 10)
        {
            PerformanceWarning?.Invoke($"检测到{data.DeadSubscriptions}个死亡事件订阅");
        }
    }

    /// <summary>
    /// 强制执行垃圾回收
    /// </summary>
    public void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// 清理所有缓存
    /// </summary>
    public void ClearAllCaches()
    {
        ImageCacheManager.Instance.Clear();
        EventSubscriptionManager.Instance.ClearAllSubscriptions();
    }

    /// <summary>
    /// 获取当前性能快照
    /// </summary>
    public PerformanceData GetCurrentSnapshot()
    {
        return CollectPerformanceData();
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _monitorTimer?.Dispose();
        _cpuCounter?.Dispose();
        _currentProcess?.Dispose();
    }

    public class PerformanceData
    {
        public DateTime Timestamp { get; set; }
        
        // 内存相关
        public long GcMemory { get; set; }
        public long WorkingSet { get; set; }
        public long PrivateMemory { get; set; }
        
        // CPU相关
        public float CpuUsage { get; set; }
        
        // GC相关
        public int GcCollections0 { get; set; }
        public int GcCollections1 { get; set; }
        public int GcCollections2 { get; set; }
        
        // 线程相关
        public int ThreadCount { get; set; }
        
        // 缓存相关
        public int ImageCacheItemCount { get; set; }
        public long ImageCacheMemoryUsage { get; set; }
        public double ImageCacheMemoryUsagePercentage { get; set; }

        // 事件相关
        public int EventSubscriptionCount { get; set; }
        public int AliveSubscriptions { get; set; }
        public int DeadSubscriptions { get; set; }
        
        // 格式化方法
        public string GetMemoryInfo()
        {
            return $"GC: {GcMemory / 1024 / 1024}MB, 工作集: {WorkingSet / 1024 / 1024}MB, 私有: {PrivateMemory / 1024 / 1024}MB";
        }
        
        public string GetCacheInfo()
        {
            return $"图像缓存: {ImageCacheItemCount}项 ({ImageCacheMemoryUsage / 1024 / 1024}MB, {ImageCacheMemoryUsagePercentage:F1}%)";
        }
        
        public string GetGcInfo()
        {
            return $"GC: Gen0={GcCollections0}, Gen1={GcCollections1}, Gen2={GcCollections2}";
        }
    }
}
