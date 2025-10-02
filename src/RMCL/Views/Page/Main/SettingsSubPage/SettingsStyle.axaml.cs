using System.Collections.Generic;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HarfBuzzSharp;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage;

public partial class SettingsStyle : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public SettingsStyle()
    {
        InitializeComponent();
        MainSettingPage.BackPage = new SettingsNavigation();
        
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Style,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsStyle());
                }
            }
        });

        IsEditMode = true;
    }

    private void BackgroundBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.BackPage = new SettingsStyle();
        MainSettingPage.Page.NavigationTo(new StyleBackground());
    }

    private void ColorBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.BackPage = new SettingsStyle();
        MainSettingPage.Page.NavigationTo(new StyleColor());
    }
}