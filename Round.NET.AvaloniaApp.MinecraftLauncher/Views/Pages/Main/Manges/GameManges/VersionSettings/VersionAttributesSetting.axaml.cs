﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules;
using Round.NET.AvaloniaApp.MinecraftLauncher.Views.Controls.Launch;
using LaunchJavaEdtion = Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Game.JavaEdtion.Launch.LaunchJavaEdtion;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.Main.Manges.GameManges.VersionSettings;

public partial class VersionAttributesSetting : UserControl
{
    public string version { get; set; }
    public VersionAttributesSetting()
    {
        InitializeComponent();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "导出启动脚本",
            InitialFileName = $"Run {version}.bat",
            DefaultExtension = "bat",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "BAT 批处理文件", Extensions = new List<string> { "bat" } }
            }
        };

        string? filePath = await saveFileDialog.ShowAsync(Core.MainWindow);

        if (!string.IsNullOrEmpty(filePath))
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            File.WriteAllBytes(filePath, Encoding.GetEncoding("GB18030").GetBytes(await LaunchJavaEdtion.GetLauncherCommand(version)));
        }
    }
}