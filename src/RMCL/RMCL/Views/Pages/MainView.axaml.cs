using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.FluentIcons;
using FluentAvalonia.UI.Controls;
using RMCL.Base.Enum.Update;
using RMCL.Controls.ControlHelper;
using RMCL.Models.Classes;
using RMCL.Update;
using RMCL.Views.Pages.Main;
using RMCL.Views.Pages.UpdateView;

namespace RMCL.Views.Pages;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Core.BottomBar = BottomBar;
        Core.ChildFrame = ChildFrame;
        BottomBar.ContentFrame = MainFrame;
        Core.ChildFrame.ShowedCallBack = () =>
        {
            Core.MainWindow.HomeButton.Content = new FluentIcon(){Icon = FluentIconSymbol.ArrowLeft20Regular,Margin = new Thickness(3)};
        };
        Core.ChildFrame.ClosedCallBack = () =>
        {
            Core.MainWindow.HomeButton.Content = "Round Minecraft Launcher";
        };

        BottomBar.RegisterNavigationItem(new BottomBarNavigationEntry()
        {
            IsDefault = true,
            Tag = "Home",
            Title = new Label() { Content = "主页" },
            IsNoButton = true,
            Page = new Home()
        });
        BottomBar.RegisterNavigationItem(new BottomBarNavigationEntry()
        {
            Tag = "Download",
            Title = new Label() { Content = "下载" },
            Page = new Download()
        });
        BottomBar.RegisterNavigationItem(new BottomBarNavigationEntry()
        {
            Tag = "Manage",
            Title = new Label() { Content = "管理" },
            Page = new Manage()
        });
        BottomBar.RegisterNavigationItem(new BottomBarNavigationEntry()
        {
            Tag = "Setting",
            Title = new Label() { Content = "设置" },
            Page = new Setting()
        });

        this.Loaded += async (sender, args) =>
        {
            var Update =
                new Update.UpdateDetect(
                    "https://api.github.com/repos/Round-Studio/Round.NET.AvaloniaApp.MinecraftLauncher/releases/latest");
            
            Update.BranchIndex = Config.Config.MainConfig.UpdateModel.Branch;
            Update.OnUpdate = (s, entry) =>
            {
                Task.Run(() =>
                {
                    var url = s;
                    if (Config.Config.MainConfig.UpdateModel.Proxy == UpdateProxy.Faster)
                    {
                        var sel = new GitHubProxySelector();
                        url = sel.GetBestProxyUrl().Replace("{url}", s);
                    }

                    Console.WriteLine(url);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        Core.ChildFrame.Show(new UpdatePage(url, entry));
                    });
                });
            };
            Update.Detect();
        };
    }
}