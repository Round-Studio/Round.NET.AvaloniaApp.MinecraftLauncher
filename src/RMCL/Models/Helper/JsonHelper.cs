using System.Text.Json;
using System.Text.Json.Serialization;

namespace RMCL.Models.Helper;

public class JsonHelper
{
    public static T GetBodyByJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}