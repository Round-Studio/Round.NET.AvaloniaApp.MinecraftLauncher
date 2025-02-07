﻿using FluentAvalonia.UI.Windowing;
using Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.AllControl;
using Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.Main;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Modules;

public class Core
{
    public static AppWindow MainWindow { get; set; }
    public static SystemTaskBox SystemTask { get; set; }
    public static SystemMessageBox SystemMessage { get; set; }
    public static Download DownloadPage { get; set; } = new();
}