using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.GameListSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainGameListPage : UserControl
{
    public MainGameListPage()
    {
        InitializeComponent();
        GameListNavigation.NavigateTo(new GameListRoot());
    }
}