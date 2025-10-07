using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Views.Page.DialogContent.Account;
using DialogHost = DialogHostAvalonia.DialogHost;

namespace RMCL.Views.Page.Main.AccountPage;

public partial class AccountView : UserControl
{
    public AccountView()
    {
        InitializeComponent();
    }

    private void AddAccount_OnClick(object? sender, RoutedEventArgs e)
    {
        DialogHost.Show(new DialogChooseAccountTypeContent());
    }
}