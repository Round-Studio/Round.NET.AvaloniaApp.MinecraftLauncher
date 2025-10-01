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
        LanguageType.Chinese => "",
        LanguageType.English => "en"
    };

    public static LanguageType GetLanguageType(string name) => name switch
    { 
        "" =>  LanguageType.Chinese,
        "en" => LanguageType.English
    };
}