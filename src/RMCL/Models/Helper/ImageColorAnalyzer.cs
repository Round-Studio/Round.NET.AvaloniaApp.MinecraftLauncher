using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace RMCL.Models.Helper;

public static class ImageColorAnalyzer
{
    public static List<Color> GetDominantColors(Bitmap bitmap, int colorCount = 5)
    {
        var colorFrequencies = new Dictionary<Color, int>();
        
        // 获取图片尺寸
        var width = (int)bitmap.Size.Width;
        var height = (int)bitmap.Size.Height;
        
        // 计算正确的stride（每行的字节数，通常是宽度*4，但可能有填充）
        int stride = width * 4;
        // 确保stride是4的倍数（对齐要求）
        if (stride % 4 != 0)
        {
            stride = (stride + 3) & ~3;
        }
        
        // 创建像素缓冲区
        var pixelBytes = new byte[height * stride];
        
        // 使用GCHandle固定数组
        GCHandle handle = GCHandle.Alloc(pixelBytes, GCHandleType.Pinned);
        try
        {
            IntPtr pointer = handle.AddrOfPinnedObject();
            bitmap.CopyPixels(new PixelRect(0, 0, width, height), 
                             pointer, 
                             height * stride, 
                             stride);
        }
        finally
        {
            if (handle.IsAllocated)
                handle.Free();
        }
        
        // 采样像素点（避免处理每个像素，提高性能）
        var stepX = Math.Max(1, width / 100);
        var stepY = Math.Max(1, height / 100);
        
        for (int y = 0; y < height; y += stepY)
        {
            for (int x = 0; x < width; x += stepX)
            {
                var index = (y * stride) + (x * 4);
                
                // 读取BGRA格式的像素数据
                var b = pixelBytes[index];
                var g = pixelBytes[index + 1];
                var r = pixelBytes[index + 2];
                var a = pixelBytes[index + 3];
                
                // 跳过透明像素
                if (a < 50) continue;
                
                var color = Color.FromRgb(r, g, b);
                
                // 过滤无效颜色
                if (IsValidColor(color))
                {
                    if (colorFrequencies.ContainsKey(color))
                        colorFrequencies[color]++;
                    else
                        colorFrequencies[color] = 1;
                }
            }
        }
        
        // 按出现频率排序并返回前N个颜色
        return colorFrequencies
            .OrderByDescending(pair => pair.Value)
            .Take(colorCount)
            .Select(pair => pair.Key)
            .ToList();
    }
    
    private static bool IsValidColor(Color color)
    {
        // 过滤掉接近白色、黑色或灰色的颜色
        var brightness = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
        var saturation = CalculateSaturation(color);
        
        return brightness > 30 && 
               brightness < 220 &&
               saturation > 0.1;
    }
    
    private static double CalculateSaturation(Color color)
    {
        var max = Math.Max(color.R, Math.Max(color.G, color.B));
        var min = Math.Min(color.R, Math.Min(color.G, color.B));
        
        if (max == 0) return 0;
        return (max - min) / (double)max;
    }
}