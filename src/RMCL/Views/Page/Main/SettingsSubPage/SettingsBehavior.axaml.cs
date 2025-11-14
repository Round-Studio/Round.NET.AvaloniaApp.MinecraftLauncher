using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage;

public partial class SettingsBehavior : UserControl
{
    public SettingsBehavior()
    {
        InitializeComponent();
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Behavior,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsBehavior());
                }
            }
        });
    }

    private void JavaBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new BehaviorJava());
    }

    private void ProgramBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new BehaviorProgram());
    }

    private void PluginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new BehaviorPlugin());
    }

    private void DiskBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new BehaviorDisk());
    }
}