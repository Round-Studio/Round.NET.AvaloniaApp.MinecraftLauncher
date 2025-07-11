using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 统一的图像缓存管理器，支持LRU清理和内存限制
/// </summary>
public class ImageCacheManager : IDisposable
{
    private static readonly Lazy<ImageCacheManager> _instance = new(() => new ImageCacheManager());
    public static ImageCacheManager Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly LinkedList<string> _lruList = new();
    private readonly object _lruLock = new();
    private readonly Timer _cleanupTimer;
    
    // 配置参数
    private const int MaxCacheSize = 100; // 最大缓存项数
    private const long MaxMemoryUsage = 100 * 1024 * 1024; // 100MB内存限制
    private const int CleanupIntervalMs = 30000; // 30秒清理间隔
    
    private long _currentMemoryUsage = 0;
    private bool _disposed = false;

    private ImageCacheManager()
    {
        _cleanupTimer = new Timer(CleanupExpiredItems, null, CleanupIntervalMs, CleanupIntervalMs);
    }

    /// <summary>
    /// 获取或创建Bitmap缓存
    /// </summary>
    public Bitmap GetOrCreateBitmap(string key, Func<Bitmap> factory, int targetWidth = -1)
    {
        if (_disposed) return factory();

        var cacheKey = targetWidth > 0 ? $"{key}_{targetWidth}" : key;
        
        if (_cache.TryGetValue(cacheKey, out var item))
        {
            UpdateLRU(cacheKey);
            return item.Bitmap;
        }

        var bitmap = factory();
        if (bitmap != null)
        {
            // 如果指定了目标宽度，进行缩放
            if (targetWidth > 0 && bitmap.PixelSize.Width != targetWidth)
            {
                var aspectRatio = (double)bitmap.PixelSize.Height / bitmap.PixelSize.Width;
                var targetHeight = (int)(targetWidth * aspectRatio);
                var resized = bitmap.CreateScaledBitmap(new Avalonia.PixelSize(targetWidth, targetHeight));
                bitmap.Dispose();
                bitmap = resized;
            }

            AddToCache(cacheKey, bitmap);
        }

        return bitmap;
    }

    /// <summary>
    /// 从资源加载Bitmap
    /// </summary>
    public Bitmap GetOrCreateBitmapFromAsset(string assetPath, int targetWidth = -1)
    {
        return GetOrCreateBitmap(assetPath, () =>
        {
            using var stream = AssetLoader.Open(new Uri(assetPath));
            return targetWidth > 0 ? Bitmap.DecodeToWidth(stream, targetWidth) : new Bitmap(stream);
        }, targetWidth);
    }

    /// <summary>
    /// 从文件加载Bitmap
    /// </summary>
    public Bitmap GetOrCreateBitmapFromFile(string filePath, int targetWidth = -1)
    {
        if (!File.Exists(filePath)) return null;

        return GetOrCreateBitmap(filePath, () =>
        {
            using var stream = File.OpenRead(filePath);
            return targetWidth > 0 ? Bitmap.DecodeToWidth(stream, targetWidth) : new Bitmap(stream);
        }, targetWidth);
    }

    /// <summary>
    /// 获取或创建SKBitmap缓存
    /// </summary>
    public SKBitmap GetOrCreateSKBitmap(string key, Func<SKBitmap> factory)
    {
        if (_disposed) return factory();

        if (_cache.TryGetValue(key, out var item) && item.SKBitmap != null)
        {
            UpdateLRU(key);
            return item.SKBitmap;
        }

        var skBitmap = factory();
        if (skBitmap != null)
        {
            AddToCache(key, skBitmap);
        }

        return skBitmap;
    }

    private void AddToCache(string key, Bitmap bitmap)
    {
        var estimatedSize = EstimateBitmapSize(bitmap);
        var item = new CacheItem(bitmap, estimatedSize);
        
        _cache.AddOrUpdate(key, item, (k, old) =>
        {
            old.Dispose();
            Interlocked.Add(ref _currentMemoryUsage, -old.EstimatedSize);
            return item;
        });

        Interlocked.Add(ref _currentMemoryUsage, estimatedSize);
        UpdateLRU(key);
        
        // 检查是否需要清理
        if (_cache.Count > MaxCacheSize || _currentMemoryUsage > MaxMemoryUsage)
        {
            Task.Run(CleanupOldItems);
        }
    }

    private void AddToCache(string key, SKBitmap skBitmap)
    {
        var estimatedSize = EstimateSKBitmapSize(skBitmap);
        var item = new CacheItem(skBitmap, estimatedSize);
        
        _cache.AddOrUpdate(key, item, (k, old) =>
        {
            old.Dispose();
            Interlocked.Add(ref _currentMemoryUsage, -old.EstimatedSize);
            return item;
        });

        Interlocked.Add(ref _currentMemoryUsage, estimatedSize);
        UpdateLRU(key);
        
        // 检查是否需要清理
        if (_cache.Count > MaxCacheSize || _currentMemoryUsage > MaxMemoryUsage)
        {
            Task.Run(CleanupOldItems);
        }
    }

    private void UpdateLRU(string key)
    {
        lock (_lruLock)
        {
            _lruList.Remove(key);
            _lruList.AddFirst(key);
        }
    }

    private void CleanupOldItems()
    {
        if (_disposed) return;

        var itemsToRemove = new List<string>();
        
        lock (_lruLock)
        {
            // 移除最旧的项目直到满足限制
            while ((_cache.Count > MaxCacheSize * 0.8 || _currentMemoryUsage > MaxMemoryUsage * 0.8) 
                   && _lruList.Count > 0)
            {
                var oldestKey = _lruList.Last.Value;
                _lruList.RemoveLast();
                itemsToRemove.Add(oldestKey);
            }
        }

        foreach (var key in itemsToRemove)
        {
            if (_cache.TryRemove(key, out var item))
            {
                Interlocked.Add(ref _currentMemoryUsage, -item.EstimatedSize);
                item.Dispose();
            }
        }
    }

    private void CleanupExpiredItems(object state)
    {
        if (_disposed) return;

        var expiredKeys = new List<string>();
        var cutoffTime = DateTime.UtcNow.AddMinutes(-10); // 10分钟过期

        foreach (var kvp in _cache)
        {
            if (kvp.Value.CreatedAt < cutoffTime)
            {
                expiredKeys.Add(kvp.Key);
            }
        }

        foreach (var key in expiredKeys)
        {
            if (_cache.TryRemove(key, out var item))
            {
                Interlocked.Add(ref _currentMemoryUsage, -item.EstimatedSize);
                item.Dispose();
                
                lock (_lruLock)
                {
                    _lruList.Remove(key);
                }
            }
        }
    }

    private static long EstimateBitmapSize(Bitmap bitmap)
    {
        return bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4; // RGBA
    }

    private static long EstimateSKBitmapSize(SKBitmap skBitmap)
    {
        return skBitmap.Width * skBitmap.Height * 4; // RGBA
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public void Clear()
    {
        foreach (var item in _cache.Values)
        {
            item.Dispose();
        }
        _cache.Clear();
        
        lock (_lruLock)
        {
            _lruList.Clear();
        }
        
        Interlocked.Exchange(ref _currentMemoryUsage, 0);
    }

    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    public CacheStats GetStats()
    {
        return new CacheStats
        {
            ItemCount = _cache.Count,
            MemoryUsage = _currentMemoryUsage,
            MaxMemoryUsage = MaxMemoryUsage,
            MaxItemCount = MaxCacheSize
        };
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _cleanupTimer?.Dispose();
        Clear();
    }

    private class CacheItem : IDisposable
    {
        public Bitmap Bitmap { get; }
        public SKBitmap SKBitmap { get; }
        public long EstimatedSize { get; }
        public DateTime CreatedAt { get; }

        public CacheItem(Bitmap bitmap, long estimatedSize)
        {
            Bitmap = bitmap;
            EstimatedSize = estimatedSize;
            CreatedAt = DateTime.UtcNow;
        }

        public CacheItem(SKBitmap skBitmap, long estimatedSize)
        {
            SKBitmap = skBitmap;
            EstimatedSize = estimatedSize;
            CreatedAt = DateTime.UtcNow;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
            SKBitmap?.Dispose();
        }
    }

    public class CacheStats
    {
        public int ItemCount { get; set; }
        public long MemoryUsage { get; set; }
        public long MaxMemoryUsage { get; set; }
        public int MaxItemCount { get; set; }

        public double MemoryUsagePercentage => MaxMemoryUsage > 0 ? (double)MemoryUsage / MaxMemoryUsage * 100 : 0;
        public double ItemCountPercentage => MaxItemCount > 0 ? (double)ItemCount / MaxItemCount * 100 : 0;
    }
}
