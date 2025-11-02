using System.Text.Json.Serialization;

namespace RMCL.Base.Entry.Config;

public class ProgramConfig
{
    [JsonPropertyName("togglePlugin")] public bool TogglePlugin { get; set; } = true;
    [JsonPropertyName("toggleOnline")] public bool ToggleOnline { get; set; } = false;
}