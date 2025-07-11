using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.SingleInstanceDetector;

public static class HttpApiClient
{
    private static readonly Lazy<HttpClient> _httpClient = new(() => CreateOptimizedHttpClient());
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _requestSemaphores = new();

    // 配置参数
    private const int DefaultTimeoutSeconds = 30;
    private const int MaxRetryAttempts = 3;
    private const int MaxConcurrentRequests = 10;

    private static HttpClient CreateOptimizedHttpClient()
    {
        var handler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = MaxConcurrentRequests,
            EnableMultipleHttp2Connections = true,
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds)
        };

        client.DefaultRequestHeaders.Add("User-Agent", "RMCL/3.0");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

        return client;
    }

    // 泛型方法：发送GET请求并反序列化结果
    public static async Task<T> GetAsync<T>(string url)
    {
        return await GetAsync<T>(url, null);
    }

    // 带请求头的版本
    public static async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers)
    {
        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));

        await semaphore.WaitAsync();
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

                using var response = await _httpClient.Value.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            });
        }
        finally
        {
            semaphore.Release();
        }
    }

    // 下载字节数组
    public static async Task<byte[]> GetByteArrayAsync(string url)
    {
        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));

        await semaphore.WaitAsync();
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                return await _httpClient.Value.GetByteArrayAsync(url);
            });
        }
        finally
        {
            semaphore.Release();
        }
    }

    // 下载字符串
    public static async Task<string> GetStringAsync(string url)
    {
        var semaphore = _requestSemaphores.GetOrAdd(GetDomain(url), _ => new SemaphoreSlim(MaxConcurrentRequests));

        await semaphore.WaitAsync();
        try
        {
            return await ExecuteWithRetry(async () =>
            {
                return await _httpClient.Value.GetStringAsync(url);
            });
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async Task<T> ExecuteWithRetry<T>(Func<Task<T>> operation)
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
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000);
                await Task.Delay(delay);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException && attempt < MaxRetryAttempts - 1)
            {
                lastException = ex;
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000);
                await Task.Delay(delay);
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
}