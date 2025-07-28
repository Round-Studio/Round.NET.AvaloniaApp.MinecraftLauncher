using System.Collections.Concurrent;
using Avalonia.Media.Imaging;

namespace RMCL.AssetsPool;

public class ImageResourcePool : IDisposable
{
    private readonly ConcurrentDictionary<string, (Bitmap image, DateTime lastAccess)> _pool = new();
    private readonly Timer _cleanupTimer;
    private readonly TimeSpan _inactiveTimeout;
    private readonly object _disposeLock = new();
    private bool _disposed;

    ~ImageResourcePool()
    {
        Dispose();
    }

    /// <summary>
    /// 清理回调
    /// </summary>
    private void CleanupCallback(object state)
    {
        lock (_disposeLock)
        {
            if (_disposed) return;

            var now = DateTime.Now;
            foreach (var kvp in _pool)
            {
                if (now - kvp.Value.lastAccess > _inactiveTimeout)
                {
                    if (_pool.TryRemove(kvp.Key, out var oldEntry))
                    {
                        oldEntry.image.Dispose();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 初始化图片资源池
    /// </summary>
    /// <param name="inactiveTimeout"> 未访问超时时间 </param>
    /// <param name="cleanupInterval"> 清理检查间隔 </param>
    public ImageResourcePool(TimeSpan inactiveTimeout, TimeSpan cleanupInterval)
    {
        _inactiveTimeout = inactiveTimeout;
        _cleanupTimer = new Timer(CleanupCallback, null, cleanupInterval, cleanupInterval);
    }

    /// <summary>
    /// 获取图片资源
    /// </summary>
    /// <param name="imagePath"> 图片路径 </param>
    /// <returns> Bitmap 对象 </returns>
    public Bitmap? GetImage(string imagePath, int width = 0)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ImageResourcePool));

        // 尝试从缓存获取
        if (_pool.TryGetValue(imagePath, out var entry))
        {
            // 更新最后访问时间
            _pool[imagePath] = (entry.image, DateTime.Now);
            return entry.image;
        }

        // 加载新图片
        var image = width != 0 ? Bitmap.DecodeToWidth(new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous), width) : new Bitmap(imagePath);
        _pool[imagePath] = (image, DateTime.Now);
        return image;
    }

    /// <summary>
    /// 预加载图片到资源池
    /// </summary>
    /// <param name="imagePath"> 图片路径 </param>
    public void PreloadImage(string imagePath)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ImageResourcePool));
        if (!_pool.ContainsKey(imagePath))
        {
            var image = new Bitmap(imagePath);
            _pool[imagePath] = (image, DateTime.Now);
        }
    }

    /// <summary>
    /// 手动清理所有未使用的资源
    /// </summary>
    public void Cleanup()
    {
        CleanupCallback(null);
    }

    /// <summary>
    /// 释放资源池
    /// </summary>
    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed) return;
            _disposed = true;

            _cleanupTimer?.Dispose();

            foreach (var kvp in _pool)
            {
                kvp.Value.image.Dispose();
            }
            _pool.Clear();
        }
        GC.SuppressFinalize(this);
    }
}