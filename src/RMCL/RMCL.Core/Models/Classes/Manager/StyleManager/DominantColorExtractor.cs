using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RMCL.Core.Models.Classes.Manager.StyleManager;

public static class DominantColorExtractor
{
    private const int TargetSize = 256;  // 减小目标处理尺寸以提高性能
    private const int BlurRadius = 12;    // 减小模糊半径
    private const int ClusterCount = 16;  // 减少聚类数量
    private const int SampleStep = 3;    // 增加采样步长以减少计算量
    private const float MinSaturation = 0.2f; // 最小饱和度阈值
    private const float MaxLightness = 0.9f;  // 最大亮度阈值
    private const float MinLightness = 0.1f;  // 最小亮度阈值

    // 对象池以减少内存分配
    private static readonly ConcurrentQueue<List<SKColor>> _colorListPool = new();
    private static readonly ConcurrentQueue<float[]> _floatArrayPool = new();

    public static SKColor GetDominantColor(SKBitmap bitmap)
    {
        if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
            return SKColors.Gray;

        try
        {
            // 1. 预处理：缩小图像尺寸
            using var resized = ResizeImage(bitmap, TargetSize);

            // 2. 应用高斯模糊（可选，为了性能可以跳过）
            // using var blurred = ApplyBlur(resized, BlurRadius);

            // 3. 采样像素颜色（直接从缩放后的图像采样）
            var samples = SamplePixels(resized, SampleStep);

            if (samples.Count == 0)
                return SKColors.Gray;

            // 4. 执行优化的K-means聚类
            var clusters = OptimizedKMeansClustering(samples, ClusterCount);

            // 5. 选择最佳主色（排除黑白灰）
            return SelectDominantColor(clusters);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"主色提取失败: {ex.Message}");
            return SKColors.Gray;
        }
    }

    private static SKBitmap ResizeImage(SKBitmap source, int targetSize)
    {
        float scale = Math.Min((float)targetSize / source.Width, 
                             (float)targetSize / source.Height);
        
        var newWidth = (int)(source.Width * scale);
        var newHeight = (int)(source.Height * scale);
        
        return source.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
    }

    private static SKBitmap ApplyBlur(SKBitmap source, int radius)
    {
        using var surface = SKSurface.Create(new SKImageInfo(source.Width, source.Height));
        using var paint = new SKPaint
        {
            ImageFilter = SKImageFilter.CreateBlur(radius, radius)
        };
        
        surface.Canvas.DrawBitmap(source, 0, 0, paint);
        surface.Canvas.Flush();
        
        var result = new SKBitmap(source.Width, source.Height);
        using var snapshot = surface.Snapshot();
        snapshot.ReadPixels(result.PeekPixels());
        return result;
    }

    private static List<SKColor> SamplePixels(SKBitmap bitmap, int step)
    {
        // 从对象池获取列表
        if (!_colorListPool.TryDequeue(out var samples))
        {
            samples = new List<SKColor>();
        }
        else
        {
            samples.Clear();
        }

        for (int y = 0; y < bitmap.Height; y += step)
        {
            for (int x = 0; x < bitmap.Width; x += step)
            {
                var color = bitmap.GetPixel(x, y);

                // 忽略透明或半透明像素
                if (color.Alpha > 128)
                {
                    samples.Add(color);
                }
            }
        }

        return samples;
    }

    private static List<Cluster> OptimizedKMeansClustering(List<SKColor> colors, int clusterCount)
    {
        if (colors.Count == 0) return new List<Cluster>();

        var random = new Random();
        var clusters = new List<Cluster>();

        // 减少聚类数量以提高性能
        var actualClusterCount = Math.Min(clusterCount, Math.Min(colors.Count, 8));

        // 初始化聚类中心（排除黑白灰）
        var candidateCenters = colors.Where(c => !IsBlackWhiteGray(c)).ToList();
        if (candidateCenters.Count == 0) candidateCenters = colors;

        for (int i = 0; i < actualClusterCount; i++)
        {
            clusters.Add(new Cluster
            {
                Center = candidateCenters[random.Next(candidateCenters.Count)],
                Members = new List<SKColor>()
            });
        }

        // 减少迭代次数以提高性能
        for (int iter = 0; iter < 5; iter++)
        {
            // 清空聚类成员
            foreach (var cluster in clusters)
                cluster.Members.Clear();

            // 分配颜色到最近的聚类
            foreach (var color in colors)
            {
                var nearest = clusters.OrderBy(c => ColorDistance(color, c.Center)).First();
                nearest.Members.Add(color);
            }

            // 重新计算聚类中心
            foreach (var cluster in clusters.Where(c => c.Members.Count > 0))
            {
                cluster.Center = AverageColor(cluster.Members);
            }
        }

        // 将列表返回到对象池
        colors.Clear();
        _colorListPool.Enqueue(colors);

        return clusters;
    }

    private static List<Cluster> KMeansClustering(List<SKColor> colors, int clusterCount)
    {
        var random = new Random();
        var clusters = new List<Cluster>();
        
        // 初始化聚类中心（排除黑白灰）
        var candidateCenters = colors.Where(c => !IsBlackWhiteGray(c)).ToList();
        if (candidateCenters.Count == 0) candidateCenters = colors;
        
        for (int i = 0; i < clusterCount && i < candidateCenters.Count; i++)
        {
            clusters.Add(new Cluster
            {
                Center = candidateCenters[random.Next(candidateCenters.Count)],
                Members = new List<SKColor>()
            });
        }
        
        // 迭代聚类
        for (int iter = 0; iter < 10; iter++)
        {
            // 清空聚类成员
            foreach (var cluster in clusters)
                cluster.Members.Clear();
            
            // 分配颜色到最近的聚类
            foreach (var color in colors)
            {
                var nearest = clusters.OrderBy(c => ColorDistance(color, c.Center)).First();
                nearest.Members.Add(color);
            }
            
            // 重新计算聚类中心
            foreach (var cluster in clusters.Where(c => c.Members.Count > 0))
            {
                cluster.Center = AverageColor(cluster.Members);
            }
        }
        
        return clusters;
    }

    private static bool IsBlackWhiteGray(SKColor color)
    {
        // 转换为HSL颜色空间判断
        var (h, s, l) = RgbToHsl(color);
        
        // 饱和度太低视为灰色
        if (s < MinSaturation) return true;
        
        // 亮度过高或过低视为白色或黑色
        if (l > MaxLightness || l < MinLightness) return true;
        
        return false;
    }

    private static (float h, float s, float l) RgbToHsl(SKColor color)
    {
        float r = color.Red / 255f;
        float g = color.Green / 255f;
        float b = color.Blue / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float h, s, l = (max + min) / 2f;

        if (max == min)
        {
            h = s = 0; // 灰色
        }
        else
        {
            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r) h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g) h = (b - r) / d + 2f;
            else h = (r - g) / d + 4f;

            h /= 6f;
        }

        return (h, s, l);
    }

    private static double ColorDistance(SKColor a, SKColor b)
    {
        // 使用感知颜色距离公式
        var rMean = (a.Red + b.Red) / 2.0;
        var r = a.Red - b.Red;
        var g = a.Green - b.Green;
        var bDiff = a.Blue - b.Blue;
        
        return Math.Sqrt(
            (2 + rMean / 256) * r * r + 
            4 * g * g + 
            (2 + (255 - rMean) / 256) * bDiff * bDiff
        );
    }

    private static SKColor AverageColor(IEnumerable<SKColor> colors)
    {
        long r = 0, g = 0, b = 0, count = 0;
        
        foreach (var color in colors)
        {
            r += color.Red;
            g += color.Green;
            b += color.Blue;
            count++;
        }
        
        if (count == 0) return SKColors.Transparent;
        
        return new SKColor(
            (byte)(r / count),
            (byte)(g / count),
            (byte)(b / count)
        );
    }

    private static SKColor SelectDominantColor(List<Cluster> clusters)
    {
        // 优先选择非黑白灰且饱和度高的颜色
        var validClusters = clusters
            .Where(c => !IsBlackWhiteGray(c.Center))
            .OrderByDescending(c => c.Members.Count)
            .ThenByDescending(c => ColorSaturation(c.Center))
            .ToList();
        
        // 如果没有符合条件的颜色，则返回原始算法结果
        if (validClusters.Count == 0)
        {
            return clusters
                .OrderByDescending(c => c.Members.Count)
                .First().Center;
        }
        
        return validClusters.First().Center;
    }

    private static double ColorSaturation(SKColor color)
    {
        // 计算颜色饱和度
        byte max = Math.Max(color.Red, Math.Max(color.Green, color.Blue));
        byte min = Math.Min(color.Red, Math.Min(color.Green, color.Blue));
        
        if (max == 0) return 0;
        return (max - min) / (double)max;
    }

    private class Cluster
    {
        public SKColor Center { get; set; }
        public List<SKColor> Members { get; set; }
    }
}