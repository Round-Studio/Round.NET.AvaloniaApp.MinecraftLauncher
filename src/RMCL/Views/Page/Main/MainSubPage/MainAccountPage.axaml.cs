using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.AccountPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainAccountPage : UserControl
{
    public MainAccountPage()
    {
        InitializeComponent();
        
        AccountNavigation.NavigateTo(new AccountView());
    }
}