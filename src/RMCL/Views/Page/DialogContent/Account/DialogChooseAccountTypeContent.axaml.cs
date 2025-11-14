using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using OverrideLauncher.Core.Base.Enum.Account;
using OverrideLauncher.Core.Classes.Account;
using RMCL.Models.Global;
using RMCL.Properties;

namespace RMCL.Views.Page.DialogContent.Account;

public partial class DialogChooseAccountTypeContent : UserControl
{
    public AccountType AccountType => (AccountType)ChooseList.SelectedIndex;
    public DialogChooseAccountTypeContent()
    {
        InitializeComponent();
    }

    public void AddAccount(AccountType accountType)
    {
        switch (accountType)
        {
            case AccountType.Microsoft:
                Console.WriteLine(@"微软账户未启用");
                break;
            case AccountType.Offline:
                Console.WriteLine(@"开始添加离线账户");
                AddOfflineAccount();
                break;
        }
    }

    private void AddOfflineAccount()
    {
        var dialog = new DialogAddOfflineAccountContent();
        DialogHost.Show(new DialogInfo()
        {
            Content = dialog,
            Title = Resource.Account_AddAccount_Offline_Title,
            CloseButtonText = Resource.Account_AddAccount_Offline_Add,
            PrimaryButtonText = Resource.Account_AddAccount_Offline_Cancel,
            AccountButton = DialogButtons.CloseButton,
            CloseAction = () =>
            {
                var accountName = dialog.AccountName;
                if (!string.IsNullOrEmpty(accountName))
                {
                    GlobalModels.Config.Data.AccountConfig.Accounts.Add(new AccountOffline(accountName).Authenticate());
                    GlobalModels.Config.Save();
                }
            }
        });
    }
}