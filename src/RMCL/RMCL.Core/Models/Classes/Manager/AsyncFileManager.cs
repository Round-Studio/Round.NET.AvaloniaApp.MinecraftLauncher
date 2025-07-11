using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 异步文件操作管理器，提供高性能的文件I/O操作
/// </summary>
public class AsyncFileManager : IDisposable
{
    private static readonly Lazy<AsyncFileManager> _instance = new(() => new AsyncFileManager());
    public static AsyncFileManager Instance => _instance.Value;

    private readonly SemaphoreSlim _fileSemaphore;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();
    private bool _disposed = false;

    // 配置参数
    private const int MaxConcurrentFileOperations = 10;
    private const int BufferSize = 8192;

    private AsyncFileManager()
    {
        _fileSemaphore = new SemaphoreSlim(MaxConcurrentFileOperations);
    }

    /// <summary>
    /// 异步读取文本文件
    /// </summary>
    public async Task<string> ReadTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));
        if (!File.Exists(filePath)) return null;

        var fileLock = GetFileLock(filePath);
        await fileLock.WaitAsync(cancellationToken);
        
        try
        {
            await _fileSemaphore.WaitAsync(cancellationToken);
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// 异步写入文本文件
    /// </summary>
    public async Task WriteTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fileLock = GetFileLock(filePath);
        await fileLock.WaitAsync(cancellationToken);
        
        try
        {
            await _fileSemaphore.WaitAsync(cancellationToken);
            try
            {
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);
                using var writer = new StreamWriter(stream, Encoding.UTF8);
                await writer.WriteAsync(content);
                await writer.FlushAsync();
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// 异步读取JSON文件并反序列化
    /// </summary>
    public async Task<T> ReadJsonAsync<T>(string filePath, CancellationToken cancellationToken = default)
    {
        var json = await ReadTextAsync(filePath, cancellationToken);
        if (string.IsNullOrEmpty(json)) return default(T);

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON反序列化失败 {filePath}: {ex.Message}");
            return default(T);
        }
    }

    /// <summary>
    /// 异步序列化并写入JSON文件
    /// </summary>
    public async Task WriteJsonAsync<T>(string filePath, T data, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            await WriteTextAsync(filePath, json, cancellationToken);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON序列化失败 {filePath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 异步读取字节数组
    /// </summary>
    public async Task<byte[]> ReadBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));
        if (!File.Exists(filePath)) return null;

        var fileLock = GetFileLock(filePath);
        await fileLock.WaitAsync(cancellationToken);
        
        try
        {
            await _fileSemaphore.WaitAsync(cancellationToken);
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                return buffer;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// 异步写入字节数组
    /// </summary>
    public async Task WriteBytesAsync(string filePath, byte[] data, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fileLock = GetFileLock(filePath);
        await fileLock.WaitAsync(cancellationToken);
        
        try
        {
            await _fileSemaphore.WaitAsync(cancellationToken);
            try
            {
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);
                await stream.WriteAsync(data, 0, data.Length, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// 异步复制文件
    /// </summary>
    public async Task CopyFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));
        if (!File.Exists(sourcePath)) throw new FileNotFoundException($"源文件不存在: {sourcePath}");

        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var sourceLock = GetFileLock(sourcePath);
        var destLock = GetFileLock(destinationPath);
        
        await sourceLock.WaitAsync(cancellationToken);
        try
        {
            await destLock.WaitAsync(cancellationToken);
            try
            {
                await _fileSemaphore.WaitAsync(cancellationToken);
                try
                {
                    using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);
                    using var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);
                    await sourceStream.CopyToAsync(destStream, BufferSize, cancellationToken);
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
            finally
            {
                destLock.Release();
            }
        }
        finally
        {
            sourceLock.Release();
        }
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// 异步删除文件
    /// </summary>
    public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AsyncFileManager));
        if (!File.Exists(filePath)) return;

        var fileLock = GetFileLock(filePath);
        await fileLock.WaitAsync(cancellationToken);
        
        try
        {
            await Task.Run(() => File.Delete(filePath), cancellationToken);
        }
        finally
        {
            fileLock.Release();
            // 清理文件锁
            _fileLocks.TryRemove(filePath, out _);
        }
    }

    private SemaphoreSlim GetFileLock(string filePath)
    {
        return _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));
    }

    /// <summary>
    /// 获取文件操作统计信息
    /// </summary>
    public FileOperationStats GetStats()
    {
        return new FileOperationStats
        {
            ActiveFileLocks = _fileLocks.Count,
            AvailableFileOperations = _fileSemaphore.CurrentCount,
            MaxConcurrentOperations = MaxConcurrentFileOperations
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _fileSemaphore?.Dispose();
        
        foreach (var fileLock in _fileLocks.Values)
        {
            fileLock.Dispose();
        }
        _fileLocks.Clear();
    }

    public class FileOperationStats
    {
        public int ActiveFileLocks { get; set; }
        public int AvailableFileOperations { get; set; }
        public int MaxConcurrentOperations { get; set; }
        
        public double OperationUtilization => MaxConcurrentOperations > 0 
            ? (double)(MaxConcurrentOperations - AvailableFileOperations) / MaxConcurrentOperations * 100 
            : 0;
    }
}
