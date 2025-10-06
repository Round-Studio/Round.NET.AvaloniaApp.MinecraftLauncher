using System;

namespace RMCL.Models;

public class BackMaterialHelper
{
    public enum BackMaterialType
    {
        None,
        CircuitBoard,
        Cellular,
        Skull
    }

    public static string GetStringName(BackMaterialType type) => type switch
    { 
        BackMaterialType.CircuitBoard => "电路板.Background.png",
        BackMaterialType.Cellular => "蜂窝.Background.png",
        BackMaterialType.Skull => "骷髅.Background.png",
        _ => String.Empty
    };
}