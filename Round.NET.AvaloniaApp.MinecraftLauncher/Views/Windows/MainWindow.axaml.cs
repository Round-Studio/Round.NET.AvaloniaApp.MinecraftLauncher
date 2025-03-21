﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        Message.Show("插件加载",$"当前已加载 {PlugsLoader.Plugs.Count} 个插件！",InfoBarSeverity.Success);
        
        Task.Run(() =>
        {
            var downloader = new Updater((v,s) =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
        
                // 获取程序集的版本信息
                Version version = assembly.GetName().Version;
                if (v.Replace("v", "").Replace("0","").Replace(".","") != version.ToString().Replace(".","").Replace("0",""))
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var con = new ContentDialog()
                        {
                            PrimaryButtonText = "取消",
                            CloseButtonText = "现在更新",
                            Title = $"更新 RMCL3 - {v.Replace("0","")}",
                            DefaultButton = ContentDialogButton.Close,
                            Content = new StackPanel()
                            {
                                Children =
                                {
                                    new Label()
                                    {
                                        Content = "你好！打扰一下~\nRMCL当前有个更新，需要花费您一些时间，请问您是否更新？"
                                    },
                                    new Label()
                                    {
                                        Content = $"当前版本：v{version.ToString().Replace(".","").Replace("0","")}"
                                    },
                                    new Label()
                                    {
                                        Content = $"更新版本：{v.Replace(".","").Replace("0","")}"
                                    }
                                }
                            }
                        };
                        con.CloseButtonClick += (_, __) =>
                        {
                            var dow = new DownloadUpdate();
                            dow.Tuid = SystemMessageTaskMange.AddTask(dow);
                            dow.URL = s;
                            dow.Version = v.Replace(".","").Replace("0","");
                            dow.Download();
                        };
                        con.ShowAsync();
                    });
                }
            });
            downloader.GetDownloadUrlAsync("https://api.github.com/repos/Round-Studio/Round.NET.AvaloniaApp.MinecraftLauncher/releases").Wait();
        });
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Config.MainConfig.WindowWidth = (int)this.Bounds.Width;
        Config.MainConfig.WindowHeight = (int)this.Bounds.Height;
        Config.SaveConfig();
    }
}
