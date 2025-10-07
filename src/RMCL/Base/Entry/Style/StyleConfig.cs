using System.Collections.Generic;
using System.Text.Json.Serialization;
using RMCL.Base.Enum.Style;
using RMCL.Models;

namespace RMCL.Base.Entry.Style;

public class StyleConfig
{
    [JsonPropertyName("backMaterialType")]
    public BackMaterialHelper.BackMaterialType BackMaterialType { get; set; } =
        BackMaterialHelper.BackMaterialType.CircuitBoard;

    [JsonPropertyName("lightThemeType")] public ThemeModelEnum LightThemeType { get; set; } = ThemeModelEnum.Dark;
    
    [JsonPropertyName("backgroundImages")] public List<string> BackgroundImages { get; set; } = new List<string>();
    [JsonPropertyName("backgroundImageSelectedIndex")] public int BackgroundImageSelectedIndex { get; set; } = -1;
    [JsonPropertyName("backgroundImageOpacity")] public int BackgroundImageOpacity { get; set; } = 100;
    [JsonPropertyName("backgroundImageBlur")] public int BackgroundImageBlur { get; set; } = 0;
    [JsonPropertyName("styleType")] public StyleType StyleType { get; set; } = StyleType.AccentColor;
}