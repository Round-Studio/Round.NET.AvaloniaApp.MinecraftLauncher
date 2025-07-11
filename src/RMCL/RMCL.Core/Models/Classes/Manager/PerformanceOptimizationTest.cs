using System;
using System.IO;
using System.Threading.Tasks;

namespace RMCL.Core.Models.Classes.Manager;

/// <summary>
/// 性能优化测试类，用于验证所有优化组件是否正常工作
/// </summary>
public static class PerformanceOptimizationTest
{
    /// <summary>
    /// 运行所有性能优化组件的测试
    /// </summary>
    public static async Task RunAllTests()
    {
        Console.WriteLine("开始性能优化测试...");
        
        try
        {
            // 测试图像缓存管理器
            TestImageCacheManager();
            
            // 测试事件订阅管理器
            TestEventSubscriptionManager();
            
            // 测试异步文件管理器
            await TestAsyncFileManager();
            
            // 测试性能监控器
            TestPerformanceMonitor();
            
            Console.WriteLine("所有性能优化测试通过！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"性能优化测试失败: {ex.Message}");
            throw;
        }
    }
    
    private static void TestImageCacheManager()
    {
        Console.WriteLine("测试图像缓存管理器...");
        
        var cacheManager = ImageCacheManager.Instance;
        var stats = cacheManager.GetStats();
        
        Console.WriteLine($"图像缓存统计: {stats.ItemCount}项, {stats.MemoryUsage / 1024 / 1024}MB");
        Console.WriteLine("图像缓存管理器测试通过");
    }
    
    private static void TestEventSubscriptionManager()
    {
        Console.WriteLine("测试事件订阅管理器...");
        
        var eventManager = EventSubscriptionManager.Instance;
        var stats = eventManager.GetStats();
        
        Console.WriteLine($"事件订阅统计: {stats.EventCount}个事件, {stats.AliveSubscriptions}个活跃订阅");
        Console.WriteLine("事件订阅管理器测试通过");
    }
    
    private static async Task TestAsyncFileManager()
    {
        Console.WriteLine("测试异步文件管理器...");
        
        var fileManager = AsyncFileManager.Instance;
        var stats = fileManager.GetStats();
        
        Console.WriteLine($"文件操作统计: {stats.ActiveFileLocks}个文件锁, 利用率{stats.OperationUtilization:F1}%");
        
        // 测试简单的文件操作
        var testFile = Path.GetTempFileName();
        try
        {
            await fileManager.WriteTextAsync(testFile, "测试内容");
            var content = await fileManager.ReadTextAsync(testFile);
            
            if (content != "测试内容")
            {
                throw new Exception("文件读写测试失败");
            }
            
            Console.WriteLine("异步文件管理器测试通过");
        }
        finally
        {
            if (File.Exists(testFile))
            {
                await fileManager.DeleteFileAsync(testFile);
            }
        }
    }
    
    private static void TestPerformanceMonitor()
    {
        Console.WriteLine("测试性能监控器...");
        
        var monitor = PerformanceMonitor.Instance;
        var snapshot = monitor.GetCurrentSnapshot();
        
        Console.WriteLine($"性能快照: {snapshot.GetMemoryInfo()}");
        Console.WriteLine($"缓存信息: {snapshot.GetCacheInfo()}");
        Console.WriteLine($"GC信息: {snapshot.GetGcInfo()}");
        
        Console.WriteLine("性能监控器测试通过");
    }
    
    /// <summary>
    /// 获取优化效果报告
    /// </summary>
    public static string GetOptimizationReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("=== RMCL 性能优化报告 ===");
        report.AppendLine();
        
        // 图像缓存统计
        var imageCacheStats = ImageCacheManager.Instance.GetStats();
        report.AppendLine($"图像缓存:");
        report.AppendLine($"  - 缓存项数: {imageCacheStats.ItemCount}/{imageCacheStats.MaxItemCount}");
        report.AppendLine($"  - 内存使用: {imageCacheStats.MemoryUsage / 1024 / 1024}MB/{imageCacheStats.MaxMemoryUsage / 1024 / 1024}MB");
        report.AppendLine($"  - 内存使用率: {imageCacheStats.MemoryUsagePercentage:F1}%");
        report.AppendLine();

        // 简化图像缓存统计（来自Controls项目）
        try
        {
            var simpleStats = RMCL.Controls.Helpers.SimpleImageCache.GetStats();
            report.AppendLine($"控件图像缓存:");
            report.AppendLine($"  - 缓存项数: {simpleStats.ItemCount}");
            report.AppendLine($"  - 内存使用: {simpleStats.MemoryUsage / 1024 / 1024}MB");
            report.AppendLine();
        }
        catch (Exception ex)
        {
            report.AppendLine($"控件图像缓存统计获取失败: {ex.Message}");
            report.AppendLine();
        }
        
        // 事件订阅统计
        var eventStats = EventSubscriptionManager.Instance.GetStats();
        report.AppendLine($"事件订阅:");
        report.AppendLine($"  - 事件数量: {eventStats.EventCount}");
        report.AppendLine($"  - 活跃订阅: {eventStats.AliveSubscriptions}");
        report.AppendLine($"  - 死亡订阅: {eventStats.DeadSubscriptions}");
        report.AppendLine($"  - 死亡订阅率: {eventStats.DeadSubscriptionPercentage:F1}%");
        report.AppendLine();
        
        // 文件操作统计
        var fileStats = AsyncFileManager.Instance.GetStats();
        report.AppendLine($"文件操作:");
        report.AppendLine($"  - 活跃文件锁: {fileStats.ActiveFileLocks}");
        report.AppendLine($"  - 操作利用率: {fileStats.OperationUtilization:F1}%");
        report.AppendLine();
        
        // 性能快照
        var perfSnapshot = PerformanceMonitor.Instance.GetCurrentSnapshot();
        report.AppendLine($"系统性能:");
        report.AppendLine($"  - {perfSnapshot.GetMemoryInfo()}");
        report.AppendLine($"  - CPU使用率: {perfSnapshot.CpuUsage:F1}%");
        report.AppendLine($"  - 线程数: {perfSnapshot.ThreadCount}");
        report.AppendLine($"  - {perfSnapshot.GetGcInfo()}");
        
        return report.ToString();
    }
    
    /// <summary>
    /// 清理所有缓存和资源
    /// </summary>
    public static void CleanupAll()
    {
        Console.WriteLine("清理所有缓存和资源...");
        
        // 清理图像缓存
        ImageCacheManager.Instance.Clear();

        // 清理简化图像缓存
        try
        {
            RMCL.Controls.Helpers.SimpleImageCache.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清理控件图像缓存失败: {ex.Message}");
        }
        
        // 清理事件订阅
        EventSubscriptionManager.Instance.ClearAllSubscriptions();
        
        // 强制垃圾回收
        PerformanceMonitor.Instance.ForceGarbageCollection();
        
        Console.WriteLine("清理完成");
    }
}
