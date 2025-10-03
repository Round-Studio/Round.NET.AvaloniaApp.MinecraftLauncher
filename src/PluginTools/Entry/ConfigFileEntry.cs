using System.Text.Json.Serialization;

namespace PluginTools.Entry;

public class ConfigFileEntry
{
    [JsonPropertyName("packName")] public string PackName { get; set; } = "";
    [JsonPropertyName("packIconPath")] public string PackIconPath { get; set; } = "";
    [JsonPropertyName("packVersion")] public string PackVersion { get; set; } = "1.0.0";
    [JsonPropertyName("packAuthor")] public string PackAuthor { get; set; } = "PluginTools";
    [JsonPropertyName("packDescription")] public string PackDescription { get; set; } = "Plugin Description";
    [JsonPropertyName("packLicense")] public string PackLicense { get; set; } = "";
    [JsonPropertyName("packLicenseUrl")] public string PackLicenseUrl { get; set; } = "";
    [JsonPropertyName("packScreenshots")] public List<string> PackScreenshots { get; set; } = new ()
    {
        "每张图片的路径"
    };

    [JsonPropertyName("buildProjectFilePath")] public string BuildProjectFilePath { get; set; } = "./Project.csproj";
    [JsonPropertyName("buildOutputPath")] public string BuildOutputPath { get; set; } = "./publish";
    [JsonPropertyName("bodyFile")] public string BodyFile { get; set; } = "Project.dll";
}