using System.Text.Json.Serialization;

namespace PluginTools.Entry;

public class PackConfig
{
    [JsonPropertyName("packName")] public string PackName { get; set; } = "";
    [JsonPropertyName("packIcon")] public string PackIconPath { get; set; } = "";
    [JsonPropertyName("packVersion")] public string PackVersion { get; set; } = "1.0.0";
    [JsonPropertyName("packAuthor")] public string PackAuthor { get; set; } = "PluginTools";
    [JsonPropertyName("packDescription")] public string PackDescription { get; set; } = "Plugin Description";
    [JsonPropertyName("packLicense")] public string PackLicense { get; set; } = "";
    [JsonPropertyName("packLicenseUrl")] public string PackLicenseUrl { get; set; } = "";
    [JsonPropertyName("bodyFile")] public string BodyFile { get; set; } = "Project.dll";
}