using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.FluentIcons;
using RMCL.Controls.Container;
using RMCL.Controls.ControlHelper;
using RMCL.Views.Pages.Main.ManagePages;
using RMCL.Views.Pages.Main.SettingPages;

namespace RMCL.Views.Pages.Main;

public partial class Setting : UserControl
{
    public Setting()
    {
        InitializeComponent();
        NavigationPage.RegisterRoute(new NavigationRouteConfig()
        {
            Route = "StyleSetting",
            Title = "个性化设置",
            Page = new StyleSetting(),
            Icon = FluentIconSymbol.StyleGuide20Regular,
        });
        NavigationPage.RegisterRoute(new NavigationRouteConfig()
        {
            Route = "DownloadSetting",
            Title = "下载设置",
            Page = new DownloadSetting(),
            Icon = FluentIconSymbol.ArrowDownload20Regular,
        });
        NavigationPage.RegisterRoute(new NavigationRouteConfig()
        {
            Route = "JavaSetting",
            Title = "Java 和内存设置",
            Page = new JavaSetting(),
            Icon = FluentIconSymbol.Memory16Regular,
        });
    }
}