namespace RMCL.Models;

public class BackMaterialHelper
{
    public enum BackMaterialType
    {
        CircuitBoard,
        Cellular
    }

    public static string GetStringName(BackMaterialType type) => type switch
    { 
        BackMaterialType.CircuitBoard => "电路板.Background.png",
        BackMaterialType.Cellular => "蜂窝.Background.png"
    };

    public static BackMaterialType GetBackMaterialType(string name) => name switch
    { 
        "电路板" =>  BackMaterialType.CircuitBoard,
        "蜂窝" => BackMaterialType.Cellular
    };
}