using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.OtherPage;

namespace RMCL.Views.Page.Main.SettingsSubPage;

public partial class SettingsNavigation : UserControl
{
    public SettingsStyle SettingsStyle = new SettingsStyle();
    public SettingsNavigation()
    {
        InitializeComponent();
    }

    private void StyleSetting_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(SettingsStyle);
    }

    private void AboutUs_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.Page.NavigationTo(new OtherAboutUs());
    }
}