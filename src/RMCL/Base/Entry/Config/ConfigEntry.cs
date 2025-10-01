using System.Text.Json.Serialization;
using RMCL.Models;

namespace RMCL.Base.Entry.Config;

public class ConfigEntry
{
    [JsonPropertyName("language")]
    public LanguageHelper.LanguageType Language { get; set; } = LanguageHelper.LanguageType.Chinese;

    [JsonPropertyName("backMaterialType")]
    public BackMaterialHelper.BackMaterialType BackMaterialType { get; set; } =
        BackMaterialHelper.BackMaterialType.CircuitBoard;
}