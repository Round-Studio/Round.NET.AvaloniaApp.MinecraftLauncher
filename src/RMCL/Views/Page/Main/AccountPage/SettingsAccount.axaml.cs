using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.AccountPage;

public partial class SettingsAccount : UserControl
{
    public SettingsAccount()
    {
        InitializeComponent();
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = "账户",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsAccount());
                }
            }
        });
    }
}