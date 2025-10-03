using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Properties;
using RMCL.Views.Page.DialogContent.Account;
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
                ItemName = Resource.Settings_Account,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsAccount());
                }
            }
        });
    }

    private void AddUserBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        DialogHost.Show(new DialogInfo()
        {
            Title = "新增账户",
            Content = new DialogContentAccountAdd(),
            CloseButtonText = "新增",
            PrimaryButtonText = "取消",
            AccountButton = DialogButtons.CloseButton
        });
    }
}