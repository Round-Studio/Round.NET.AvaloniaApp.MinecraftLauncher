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

    /// <summary>
    /// 从 Avalonia 资源中读取文本内容
    /// </summary>
    /// <param name="avaresUri">资源URI，例如：avares://RMCL/Assets/Text/file.txt</param>
    /// <returns>文本内容</returns>
    public static async Task<string> GetTextFromResource(string avaresUri)
    {
        using var stream = AssetLoader.Open(new Uri(avaresUri));
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// 从 Avalonia 资源中读取文本内容（同步版本）
    /// </summary>
    /// <param name="avaresUri">资源URI</param>
    /// <returns>文本内容</returns>
    public static string GetTextFromResourceSync(string avaresUri)
    {
        using var stream = AssetLoader.Open(new Uri(avaresUri));
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// 从 Avalonia 资源中按行读取文本内容
    /// </summary>
    /// <param name="avaresUri">资源URI</param>
    /// <returns>文本行数组</returns>
    public static async Task<string[]> GetLinesFromResource(string avaresUri)
    {
        var content = await GetTextFromResource(avaresUri);
        return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }

    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="avaresUri">资源URI</param>
    /// <returns>是否存在</returns>
    public static bool ResourceExists(string avaresUri)
    {
        try
        {
            using var stream = AssetLoader.Open(new Uri(avaresUri));
            return stream != null && stream.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取资源流（用于更复杂的操作）
    /// </summary>
    /// <param name="avaresUri">资源URI</param>
    /// <returns>资源流</returns>
    public static Stream GetResourceStream(string avaresUri)
    {
        return AssetLoader.Open(new Uri(avaresUri));
    }
}