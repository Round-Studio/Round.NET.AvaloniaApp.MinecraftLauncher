using System.Text.Json.Serialization;

namespace RMCL.Base.Entry.Config;

public class ConfigEntry
{
    [JsonPropertyName("windowInfo")] public WindowPoint WindowInfo { get; set; } = new WindowPoint();
    [JsonPropertyName("firstRun")] public bool FirstRun { get; set; } = true;
}