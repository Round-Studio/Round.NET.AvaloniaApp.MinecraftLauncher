using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Properties;
using RMCL.Views.Page.DialogContent.Account;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupAccount : UserControl
{
    public SetupAccount()
    {
        InitializeComponent();
    }

    private void AddNewAccount_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new DialogChooseAccountTypeContent();
        var dialoginfo = new DialogInfo()
        {
            Content = dialog,
            Title = Resource.Account_AddAccount_Title,
            CloseButtonText = Resource.Account_AddAccount_OK,
            PrimaryButtonText = Resource.Account_AddAccount_Cancel,
            AccountButton = DialogButtons.CloseButton,
            CloseAction = () =>
            {
                var accountType = dialog.AccountType;
                Console.WriteLine($@"新增用户：{accountType}");
                dialog.AddAccount(accountType);
            }
        };
        
        DialogHost.Show(dialoginfo);
    }
}