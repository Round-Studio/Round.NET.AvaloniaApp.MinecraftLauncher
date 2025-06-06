﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using RMCL.Models.Classes.Manager.StyleManager;

namespace RMCL.Views.Pages.Main.SettingPages.SettingSubPages;

public partial class ColorGlassSetting : UserControl
{
    public bool IsEdit { get; set; } = false;
    public ColorGlassSetting()
    {
        InitializeComponent();
        this.ColorPicker.Color = Color.Parse(Config.Config.MainConfig.Background.ColorGlassEntry.HtmlColor);
        IsEdit = true;
    }

    private void ColorView_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (IsEdit)
        {
            Config.Config.MainConfig.Background.ColorGlassEntry.HtmlColor = this.ColorPicker.Color.ToString();
            Config.Config.SaveConfig();
            
            StyleManager.UpdateBackground();
        }
    }
}