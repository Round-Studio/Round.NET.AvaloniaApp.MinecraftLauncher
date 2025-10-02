using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Views.Page.Main.MainSubPage;
using RMCL.Views.Page.Main.SettingsSubPage;

namespace RMCL.Views.Page.Main.OtherPage;

public partial class OtherAboutUs : UserControl
{
    public OtherAboutUs()
    {
        InitializeComponent();
        MainSettingPage.BackPage = new SettingsNavigation();
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = "关于",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new OtherAboutUs());
                }
            }
        });
    }

    private void OpenSourceBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainSettingPage.BackPage = new OtherAboutUs();
        MainSettingPage.Page.NavigationTo(new OtherOpenSourceProjects());
    }
}