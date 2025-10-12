using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;

namespace RMCL.Models.Helper;

public class AvaResHelper
{
    public static async Task ExtractAvaresResource(string avaresUri, string outputFilePath)
    {
        // 确保目标目录存在
        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var stream = AssetLoader.Open(new Uri(avaresUri)))
        {
            using (var fileStream = File.Create(outputFilePath))
            {
                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync(); // 确保数据完全写入
            }
        } // 这里两个流都会自动释放
    }
}