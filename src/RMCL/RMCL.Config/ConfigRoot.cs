using System.Collections.Generic;
using RMCL.Base.Entry.Game.Client;
using RMCL.Base.Entry.Skin;
using RMCL.Base.Entry.Style;
using RMCL.Base.Entry.Update;

namespace RMCL.Config;

public class ConfigRoot
{
    public List<GameFolderConfig> GameFolders { get; set; } = new()
    {
        new GameFolderConfig()
        {
            Name = "启动器目录",
            Path = Path.GetFullPath(PathsDictionary.PathDictionary.DefaultGameFolder)
        }
    };

    public ClientConfig PublicClietConfig { get; set; } = new();
    public BackgroundEntry Background { get; set; } = new();
    public ColorEntry ThemeColors { get; set; } = new();
    public ButtonStyle ButtonStyle { get; set; } = new();
    public BackMusicEntry BackMusicEntry { get; set; } = new();
    public FontsConfigEntry FontsConfig { get; set; } = new();
    public SkinRenderEntry SkinRenderEntry { get; set; } = new();
    public bool FirstLauncher { get; set; } = true;
    public int SelectedGameFolder { get; set; } = 0;
    public int WindowWidth { get; set; } = 850;
    public int WindowHeight { get; set; } = 450;
    public int DownloadThreads { get; set; } = Math.Min(Environment.ProcessorCount * 4, 32); // 优化下载线程数
    public int WindowX { get; set; } = 0;
    public int WindowY { get; set; } = 0;
    public UpdateConfigEntry UpdateModel { get; set; } = new();
    public bool LauncherWindowTopMost { get; set; } = false;
}

public class GameFolderConfig
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int SelectedGameIndex { get; set; } = 0;
}