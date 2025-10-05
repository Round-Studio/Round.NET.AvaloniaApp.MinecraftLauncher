using System.Text.Json.Serialization;
using RMCL.Base.Enum.Style;
using RMCL.Models;

namespace RMCL.Base.Entry.Style;

public class StyleConfig
{
    [JsonPropertyName("backMaterialType")]
    public BackMaterialHelper.BackMaterialType BackMaterialType { get; set; } =
        BackMaterialHelper.BackMaterialType.CircuitBoard;

    [JsonPropertyName("themeType")] public ThemeModelEnum ThemeType { get; set; } = ThemeModelEnum.Dark;
}