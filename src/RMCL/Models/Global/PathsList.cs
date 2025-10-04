using System;
using System.IO;

namespace RMCL.Models.Global;

public class PathsList
{
    public static readonly string RootConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RoundStudio", "RMCL4");

    public static readonly string ConfigPath = Path.Combine(RootConfigPath, "RMCL.Config", "Config.json");
    public static readonly string LogPath = Path.Combine(RootConfigPath, "RMCL.Log");
    public static readonly string TempPath = Path.Combine(RootConfigPath, "RMCL.Temp");
    public static readonly string PluginPath = Path.Combine(RootConfigPath, "RMCL.Plugin");
    public static readonly string PluginTempPath = Path.Combine(TempPath, "Plugin.Temp");
}