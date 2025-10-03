namespace RMCL.Models;

public class LanguageHelper
{
    public enum LanguageType
    {
        Chinese,
        English
    }

    public static string GetStringName(LanguageType type) => type switch
    { 
        LanguageType.Chinese => "zh-Hans",
        LanguageType.English => "en",
        _ => string.Empty
    };

    public static LanguageType GetLanguageType(string name) => name switch
    { 
        "zh-Hans" =>  LanguageType.Chinese,
        "en" => LanguageType.English,
        _ => LanguageType.Chinese
    };
}