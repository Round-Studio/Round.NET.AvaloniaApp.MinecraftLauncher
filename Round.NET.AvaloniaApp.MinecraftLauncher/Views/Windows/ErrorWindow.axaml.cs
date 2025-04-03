﻿using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Classes;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Config;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Entry;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.ExceptionMessage;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Message;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Views.Windows;

public partial class ErrorWindow : Window
{
    public ExceptionEntry ExceptionEntry { get; set; } = new();
    public ErrorWindow()
    {
        InitializeComponent();
        TimeShow.Content = DateTime.Now.ToString();
    }

    public void Show(Exception ex)
    {
        ErrorTypeLabel.Content = ex.GetType().Name;
        StackMessage.Text = ex.ToString();
        ErrorMessage.Text = ex.Message;
        
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        var sysstr = new SequenceString();
        sysstr.Add(new() { Title = "操作系统版本", Text = Environment.OSVersion.ToString() });
        sysstr.Add(new() { Title = "64位操作系统", Text = Environment.Is64BitOperatingSystem.ToString() });
        sysstr.Add(new() { Title = "系统语言", Text = CultureInfo.InstalledUICulture.Name });
        sysstr.Add(new() { Title = "时区", Text = localTimeZone.DisplayName });
        SystemMessage.Text = sysstr.GetResult();
        
        var constr = new SequenceString();
        constr.Add(new() { Title = "当前游戏目录", Text = Config.MainConfig.GameFolders[Config.MainConfig.SelectedGameFolder].Path });
        constr.Add(new() { Title = "当前选择游戏", Text = Path.GetFileName(Directory.GetDirectories(Config.MainConfig.GameFolders[Config.MainConfig.SelectedGameFolder].Path+"/versions")[Config.MainConfig.GameFolders[Config.MainConfig.SelectedGameFolder].SelectedGameIndex]) });
        constr.Add(new() { Title = "背景显示模式", Text = Config.MainConfig.BackModlue.ToString() });
        constr.Add(new() { Title = "虚拟机内存", Text = Config.MainConfig.JavaUseMemory.ToString() });
        constr.Add(new() { Title = "最大下载线程", Text = Config.MainConfig.DownloadThreads.ToString() });
        ConfigMessage.Text = constr.GetResult();

        ExceptionEntry.Exception = ex.Message;
        ExceptionEntry.ExceptionSource = ex.Source;
        ExceptionEntry.StackTrace = ex.StackTrace;
        ExceptionEntry.RecordTime = DateTime.Now;
        ExceptionEntry.SystemLanguage = CultureInfo.InstalledUICulture.Name;
        ExceptionEntry.SystemVersion = Environment.OSVersion.ToString();
        ExceptionEntry.SystemTimeZone = localTimeZone.DisplayName;
        ExceptionEntry.ExceptionType = ExceptionMessage.GetExceptionSeverity(ex);

        ExceptionMessage.LogException(ExceptionEntry);
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var con = new ContentDialog()
        {
            Title = "重置",
            Content = ""
        };
    }
}