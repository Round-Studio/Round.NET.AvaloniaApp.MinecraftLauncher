using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Views.Page.Main.AccountPage;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.OtherPage;

namespace RMCL.Views.Page.Main.SettingsSubPage;

public partial class SettingsNavigation : UserControl
{
    public SettingsNavigation()
    {
        InitializeComponent();
        // MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>() {});
    }

    private void StyleSetting_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new SettingsStyle());
    }

    private void AboutUs_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new OtherAboutUs());
    }

    private void LanguageBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new SettingsLanguage());
    }

    private void AccountBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new SettingsAccount());
    }

    private void BehaviorBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new SettingsBehavior());
    }
}