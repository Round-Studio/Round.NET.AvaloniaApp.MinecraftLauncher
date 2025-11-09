using System.Text.Json.Serialization;
using RMCL.Base.Entry.Style;
using RMCL.Base.Enum.Style;
using RMCL.Models;

namespace RMCL.Base.Entry.Config;

public class ConfigEntry
{
    [JsonPropertyName("language")] public LanguageHelper.LanguageType Language { get; set; } = LanguageHelper.LanguageType.Chinese;
    [JsonPropertyName("windowInfo")] public WindowPoint WindowInfo { get; set; } = new WindowPoint();
    [JsonPropertyName("styleConfig")] public StyleConfig StyleConfig { get; set; } = new StyleConfig();
    [JsonPropertyName("accountConfig")] public AccountConfig AccountConfig { get; set; } = new AccountConfig();
    [JsonPropertyName("programConfig")] public ProgramConfig ProgramConfig { get; set; } = new ProgramConfig();
    [JsonPropertyName("firstRun")] public bool FirstRun { get; set; } = true;
}