using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.View;
using OverrideLauncher.Core.Base.Enum.Account;
using RMCL.Models.Global;
using RMCL.Views.Control.Item;
using RMCL.Views.Page.DialogContent.Account;

namespace RMCL.Views.Page.Main.AccountPage;

public partial class AccountView : UserControl
{
    public AccountView()
    {
        InitializeComponent();

        UpdateUI();
    }

    public void UpdateUI()
    {
        GlobalModels.Config.Data.AccountConfig.Accounts.ForEach(user =>
        {
            AccountsView.Items.Add(new ItemViewItem()
            {
                Content = new AccountChooseItem(user),
            });
        });
        if (GlobalModels.Config.Data.AccountConfig.Accounts.Count == 0)
        {
            NoneBox.IsVisible = true;
            AccountsView.IsVisible = false;
        }
        else
        {
            NoneBox.IsVisible = false;
            AccountsView.IsVisible = true;
        }
    }
    
    private void AddAccount_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new DialogChooseAccountTypeContent();
        var dialoginfo = new DialogInfo()
        {
            Content = dialog,
            Title = "选择账户类型",
            CloseButtonText = "确定",
            PrimaryButtonText = "取消",
            AccountButton = DialogButtons.CloseButton,
            CloseAction = () =>
            {
                var accountType = dialog.AccountType;
                Console.WriteLine($"新增用户：{accountType}");
                dialog.AddAccount(accountType);
            }
        };
        
        DialogHost.Show(dialoginfo);
    }
}