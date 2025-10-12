using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using RMCL.Views.Page.Main.OnlineSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainOnlinePage : UserControl
{
    public static NavigationFrame NavigationFrame;

    public MainOnlinePage()
    {
        InitializeComponent();

        Task.Run(async () =>
        {
            if (!Directory.Exists(PathsList.OnlinePath)) Directory.CreateDirectory(PathsList.OnlinePath);

            await AvaResHelper.ExtractAvaresResource("avares://RMCL/Assets/EasyTier.zip",
                Path.Combine(PathsList.TempPath, "EasyTier.zip"));

            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(PathsList.TempPath, "EasyTier.zip"),
                PathsList.OnlinePath, true);
        });

        NavigationFrame = this.OnlineNavigation;
        BreadcrumbBar.RootItemClick = () => { };

        if (OnlineHost.OnlineService != null)
        {
            this.OnlineNavigation.NavigateTo(new OnlineHost(OnlineHost.OnlineService));
        }else if (OnlineLink.OnlineService != null)
        {
            this.OnlineNavigation.NavigateTo(new OnlineLink(OnlineHost.OnlineService));
        }
        else this.OnlineNavigation.NavigateTo(new OnlineNavigation());
    }
}