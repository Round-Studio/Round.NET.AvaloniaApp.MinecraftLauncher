using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using RMCL.Views.Page.Main.OnlineSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainOnlinePage : UserControl
{
    public static NavigationFrame NavigationFrame;
    public MainOnlinePage()
    {
        InitializeComponent();

        NavigationFrame = this.OnlineNavigation;
        BreadcrumbBar.RootItemClick = () => { };
        this.OnlineNavigation.NavigateTo(new OnlineNavigation());
    }
}