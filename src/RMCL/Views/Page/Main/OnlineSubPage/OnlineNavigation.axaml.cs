using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.OnlineSubPage;

public partial class OnlineNavigation : UserControl
{
    public OnlineNavigation()
    {
        InitializeComponent();
    }

    private void Host_OnClick(object? sender, RoutedEventArgs e)
    {
        MainOnlinePage.NavigationFrame.NavigateTo(new OnlineHostChooseGame());
    }
}