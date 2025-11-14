using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorDisk : UserControl
{
    public BehaviorDisk()
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
            },
            new BreadcrumbItemInfo()
            {
                ItemName = "储存感知",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new BehaviorDisk());
                }
            }
        });
    }
}