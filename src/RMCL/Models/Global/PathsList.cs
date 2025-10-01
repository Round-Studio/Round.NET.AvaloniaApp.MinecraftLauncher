using System;
using System.IO;

namespace RMCL.Models.Global;

public class PathsList
{
    public static readonly string RootConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RoundStudio", "RMCL4");

    public static readonly string ConfigPath = Path.Combine(RootConfigPath, "RMCL.Config", "Config.json");
}