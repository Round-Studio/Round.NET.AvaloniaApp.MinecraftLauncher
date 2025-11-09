namespace RMCL.Models;

public class LanguageHelper
{
    public enum LanguageType
    {
        Chinese,
        English,
        Japanese
    }

    public static string GetStringName(LanguageType type) => type switch
    { 
        LanguageType.Chinese => "zh-Hans",
        LanguageType.English => "en",
        LanguageType.Japanese => "ja-jp",
        _ => string.Empty
    };

    public static LanguageType GetLanguageType(string name) => name switch
    { 
        "zh-Hans" =>  LanguageType.Chinese,
        "en" => LanguageType.English,
        "ja-jp" => LanguageType.Japanese,
        _ => LanguageType.Chinese
    };
}