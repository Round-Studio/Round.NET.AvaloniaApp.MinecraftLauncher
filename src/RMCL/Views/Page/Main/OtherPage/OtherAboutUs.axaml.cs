using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.SettingsSubPage;

namespace RMCL.Views.Page.Main.OtherPage;

public partial class OtherAboutUs : UserControl
{
    public OtherAboutUs()
    {
        InitializeComponent();
        MainSettingPage.BackPage = new SettingsNavigation();
    }

    private void OpenSourceBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.BackPage = new OtherAboutUs();
        MainSettingPage.Page.NavigationTo(new OtherOpenSourceProjects());
    }
}