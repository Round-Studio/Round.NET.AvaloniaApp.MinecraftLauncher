using System.Text.Json.Serialization;

namespace RMCL.Base.Entry.Config;

public class WindowPoint
{
    [JsonPropertyName("x")] public int X { get; set; } = -1;
    [JsonPropertyName("y")] public int Y { get; set; } = -1;
    [JsonPropertyName("width")] public double Width { get; set; } = 1010;
    [JsonPropertyName("height")] public double Height { get; set; } = 630;
}