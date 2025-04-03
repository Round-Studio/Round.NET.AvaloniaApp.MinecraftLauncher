﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules;
using Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.Main.Exceptions;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.Main.Settings;

public partial class SafeSetting : UserControl
{
    public SafeSetting()
    {
        InitializeComponent();
    }

    private void ExceptionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ((MainView)Core.MainWindow.Content).CortentFrame.Content = new ExceptionPage();
        ((MainView)Core.MainWindow.Content).CortentFrame.Opacity = 1;
        ((MainView)Core.MainWindow.Content).MainCortent.Opacity = 0;
        
        Core.NavigationBar.Opacity = 0;
    }
}