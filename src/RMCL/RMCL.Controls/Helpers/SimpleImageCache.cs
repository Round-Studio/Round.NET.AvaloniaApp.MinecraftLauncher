using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace RMCL.Controls.Helpers;

/// <summary>
/// 简化的图像缓存，专门用于 RMCL.Controls 项目
/// </summary>
public static class SimpleImageCache
{
    private static readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private static readonly Timer _cleanupTimer;
    private static long _currentMemoryUsage = 0;
    
    // 配置参数
    private const int MaxCacheSize = 50; // 最大缓存项数
    private const long MaxMemoryUsage = 50 * 1024 * 1024; // 50MB内存限制
    private const int CleanupIntervalMs = 60000; // 1分钟清理间隔
    
    static SimpleImageCache()
    {
        _cleanupTimer = new Timer(CleanupExpiredItems, null, CleanupIntervalMs, CleanupIntervalMs);
    }

    /// <summary>
    /// 从资源获取或创建Bitmap
    /// </summary>
    public static Bitmap GetOrCreateBitmapFromAsset(string assetPath, int targetWidth = -1)
    {
        var cacheKey = targetWidth > 0 ? $"{assetPath}_{targetWidth}" : assetPath;
        
        if (_cache.TryGetValue(cacheKey, out var item))
        {
            item.LastAccessed = DateTime.UtcNow;
            return item.Bitmap;
        }

        try
        {
            using var stream = AssetLoader.Open(new Uri(assetPath));
            var bitmap = targetWidth > 0 ? Bitmap.DecodeToWidth(stream, targetWidth) : new Bitmap(stream);
            
            if (bitmap != null)
            {
                AddToCache(cacheKey, bitmap);
            }
            
            return bitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载资源图像失败 {assetPath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 从文件获取或创建Bitmap
    /// </summary>
    public static Bitmap GetOrCreateBitmapFromFile(string filePath, int targetWidth = -1)
    {
        if (!File.Exists(filePath)) return null;

        var cacheKey = targetWidth > 0 ? $"{filePath}_{targetWidth}" : filePath;
        
        if (_cache.TryGetValue(cacheKey, out var item))
        {
            item.LastAccessed = DateTime.UtcNow;
            return item.Bitmap;
        }

        try
        {
            using var stream = File.OpenRead(filePath);
            var bitmap = targetWidth > 0 ? Bitmap.DecodeToWidth(stream, targetWidth) : new Bitmap(stream);
            
            if (bitmap != null)
            {
                AddToCache(cacheKey, bitmap);
            }
            
            return bitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载文件图像失败 {filePath}: {ex.Message}");
            return null;
        }
    }

    private static void AddToCache(string key, Bitmap bitmap)
    {
        var estimatedSize = EstimateBitmapSize(bitmap);
        var cacheItem = new CacheItem(bitmap, estimatedSize);
        
        _cache.AddOrUpdate(key, cacheItem, (k, old) =>
        {
            Interlocked.Add(ref _currentMemoryUsage, -old.EstimatedSize);
            old.Bitmap?.Dispose();
            return cacheItem;
        });

        Interlocked.Add(ref _currentMemoryUsage, estimatedSize);
        
        // 检查是否需要清理
        if (_cache.Count > MaxCacheSize || _currentMemoryUsage > MaxMemoryUsage)
        {
            CleanupOldItems();
        }
    }

    private static void CleanupOldItems()
    {
        var itemsToRemove = new List<string>();
        var cutoffTime = DateTime.UtcNow.AddMinutes(-5); // 5分钟未使用的项目
        
        foreach (var kvp in _cache)
        {
            if (kvp.Value.LastAccessed < cutoffTime)
            {
                itemsToRemove.Add(kvp.Key);
            }
        }

        // 如果还是太多，按最后访问时间排序移除最旧的
        if (_cache.Count - itemsToRemove.Count > MaxCacheSize * 0.8)
        {
            var sortedItems = _cache
                .Where(kvp => !itemsToRemove.Contains(kvp.Key))
                .OrderBy(kvp => kvp.Value.LastAccessed)
                .Take(_cache.Count - (int)(MaxCacheSize * 0.8))
                .Select(kvp => kvp.Key);
            
            itemsToRemove.AddRange(sortedItems);
        }

        foreach (var key in itemsToRemove)
        {
            if (_cache.TryRemove(key, out var item))
            {
                Interlocked.Add(ref _currentMemoryUsage, -item.EstimatedSize);
                item.Bitmap?.Dispose();
            }
        }
    }

    private static void CleanupExpiredItems(object state)
    {
        CleanupOldItems();
    }

    private static long EstimateBitmapSize(Bitmap bitmap)
    {
        return bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4; // RGBA
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public static void Clear()
    {
        foreach (var item in _cache.Values)
        {
            item.Bitmap?.Dispose();
        }
        _cache.Clear();
        Interlocked.Exchange(ref _currentMemoryUsage, 0);
    }

    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    public static (int ItemCount, long MemoryUsage) GetStats()
    {
        return (_cache.Count, _currentMemoryUsage);
    }

    private class CacheItem
    {
        public Bitmap Bitmap { get; }
        public long EstimatedSize { get; }
        public DateTime CreatedAt { get; }
        public DateTime LastAccessed { get; set; }

        public CacheItem(Bitmap bitmap, long estimatedSize)
        {
            Bitmap = bitmap;
            EstimatedSize = estimatedSize;
            CreatedAt = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
        }
    }
}
