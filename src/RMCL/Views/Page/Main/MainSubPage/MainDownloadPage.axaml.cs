using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Views.Page.Main.DownloadPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainDownloadPage : UserControl
{
    public static MainDownloadPage? Instance { get; private set; }
    public MainDownloadPage()
    {
        Instance = this;
        InitializeComponent();
        
        BreadcrumbBar.RootItemClick = () =>
        {
            DownloadFrame.NavigateTo(new DownloadRecommend());
        };
        DownloadFrame.NavigateTo(new DownloadRecommend());
    }
}