using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 优化的HTTP客户端管理器，支持连接池、超时、重试等功能
/// </summary>
public class OptimizedHttpClientManager : IDisposable
{
    private static readonly Lazy<OptimizedHttpClientManager> _instance = new(() => new OptimizedHttpClientManager());
    public static OptimizedHttpClientManager Instance => _instance.Value;

    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _requestSemaphores = new();
    private bool _disposed = false;

    // 配置参数
    private const int DefaultTimeoutSeconds = 30;
    private const int MaxRetryAttempts = 3;
    private const int MaxConcurrentRequests = 10;
    private const int ConnectionLifetimeMinutes = 5;

    private OptimizedHttpClientManager()
    {
        var handler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(ConnectionLifetimeMinutes),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = MaxConcurrentRequests,
            EnableMultipleHttp2Connections = true,
            UseCookies = false, // 禁用Cookie以提高性能
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds)
        };

        // 设置默认请求头
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "RMCL/3.0");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
    }

    /// <summary>
    /// 发送GET请求并反序列化结果，支持重试和超时
    /// </summary>
    public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        return await GetAsync<T>(url, null, cancellationToken);
    }

    /// <summary>
    /// 发送带请求头的GET请求并反序列化结果
    /// </summary>
    public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedHttpClientManager));

        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));
        
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<T>(content);
            }, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// 下载字节数组
    /// </summary>
    public async Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedHttpClientManager));

        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));
        
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                return await _httpClient.GetByteArrayAsync(url, cancellationToken);
            }, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// 下载字符串内容
    /// </summary>
    public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedHttpClientManager));

        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));
        
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                return await _httpClient.GetStringAsync(url, cancellationToken);
            }, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// 发送POST请求
    /// </summary>
    public async Task<T> PostAsync<T>(string url, HttpContent content, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedHttpClientManager));

        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));
        
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                using var response = await _httpClient.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<T>(responseContent);
            }, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// 检查URL是否可访问
    /// </summary>
    public async Task<bool> IsUrlAccessibleAsync(string url, CancellationToken cancellationToken = default)
    {
        if (_disposed) return false;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> operation, CancellationToken cancellationToken)
    {
        Exception lastException = null;
        
        for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (HttpRequestException ex) when (attempt < MaxRetryAttempts - 1)
            {
                lastException = ex;
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000); // 指数退避
                await Task.Delay(delay, cancellationToken);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException && attempt < MaxRetryAttempts - 1)
            {
                lastException = ex;
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000);
                await Task.Delay(delay, cancellationToken);
            }
        }

        throw new HttpRequestException($"请求失败，已重试 {MaxRetryAttempts} 次", lastException);
    }

    private static string GetDomain(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Host;
        }
        catch
        {
            return "default";
        }
    }

    /// <summary>
    /// 获取连接池统计信息
    /// </summary>
    public ConnectionPoolStats GetConnectionPoolStats()
    {
        return new ConnectionPoolStats
        {
            ActiveDomains = _requestSemaphores.Count,
            MaxConcurrentRequests = MaxConcurrentRequests,
            ConnectionLifetimeMinutes = ConnectionLifetimeMinutes
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _httpClient?.Dispose();
        
        foreach (var semaphore in _requestSemaphores.Values)
        {
            semaphore.Dispose();
        }
        _requestSemaphores.Clear();
    }

    public class ConnectionPoolStats
    {
        public int ActiveDomains { get; set; }
        public int MaxConcurrentRequests { get; set; }
        public int ConnectionLifetimeMinutes { get; set; }
    }
}
