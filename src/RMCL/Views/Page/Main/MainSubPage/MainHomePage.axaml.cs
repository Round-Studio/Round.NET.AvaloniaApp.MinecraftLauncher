using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainHomePage : UserControl
{
    public MainHomePage()
    {
        InitializeComponent();
    }

    private void NavigationBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        GlobalModels.MainPageContent.Content = new MainNavigationPage();
    }
}