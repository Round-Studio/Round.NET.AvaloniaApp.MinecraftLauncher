﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using MinecraftLaunch;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Config;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Game.User;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Java;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Message;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Plugs;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Server;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.TaskMange.SystemMessage;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.UIControls;
using Round.NET.AvaloniaApp.MinecraftLauncher.Views.Controls.Download;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        User.LoadUser();
        MinecraftLauncher.Modules.Java.FindJava.JavasList.Clear();
        Config.LoadConfig();
        ServerMange.Load();
        
        DownloadMirrorManager.MaxThread = Config.MainConfig.DownloadThreads;
        DownloadMirrorManager.IsEnableMirror = false;
        
        InitializeComponent();
        PlugsLoader.LoadingPlug();
        TitleBar.Height = 38;
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        
        Directory.CreateDirectory(Path.GetFullPath("../RMCL/RMCL.Minecraft"));
        Core.MainWindow = this;
        
        try
        {
            StyleMange.Load();
        }
        catch(Exception ex)
        {
            Message.Show("主题加载",$"主题加载错误！\n{ex.Message}",InfoBarSeverity.Error);
        }

        this.Width = Config.MainConfig.WindowWidth;
        this.Height = Config.MainConfig.WindowHeight;
        if (Config.MainConfig.WindowX != 0 && Config.MainConfig.WindowY != 0)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(Config.MainConfig.WindowX, Config.MainConfig.WindowY);
        }
        Message.Show("插件加载",$"当前已加载 {PlugsLoader.Plugs.Count} 个插件！",InfoBarSeverity.Success);
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Config.MainConfig.WindowWidth = (int)this.Bounds.Width;
        Config.MainConfig.WindowHeight = (int)this.Bounds.Height;
        Config.MainConfig.WindowX = (int)this.Position.X;
        Config.MainConfig.WindowY = (int)this.Position.Y;
        Config.SaveConfig();
    }
}
