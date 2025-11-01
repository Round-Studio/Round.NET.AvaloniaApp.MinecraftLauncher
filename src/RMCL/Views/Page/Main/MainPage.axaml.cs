using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Round.SDK.Entry.RMCL;
using RMCL.Models.Global;
using RMCL.Models.Plugin;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Plugin.RMCL.Register;

namespace RMCL.Views.Page.Main;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();

        GlobalModels.MainPageContent = this.MainPageContent;
        MainPageContent.NavigateTo(new MainHomePage());

        this.LeftBar.OnNavigation = tag =>
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            object? page = Activator.CreateInstance(tag);
            if(page == null) throw new NullReferenceException(nameof(page));

            if (tag == typeof(MainHomePage))
            {
                CenterBorder.Margin = new Thickness(12);
                LaunchBtn.Margin = new Thickness(12);
                LaunchBtn.Width = 200;
                LaunchText.IsVisible = true;
                LeftBarBackground.Width = 0;
                BarBackground.Opacity = 0.6;
                BtnBackground.Opacity = 0.6;
            }
            else
            {
                CenterBorder.Margin = new Thickness(0);
                LaunchBtn.Margin = new Thickness(0,12);
                LaunchBtn.Width = 74;
                LaunchText.IsVisible = false;
                LeftBarBackground.Width = 74;
                BarBackground.Opacity = 0;
                BtnBackground.Opacity = 0;
            }
            
            MainPageContent.NavigateTo(page);
        };

        RegisterService.API.RegisterBottomBarItem =
            info => Dispatcher.UIThread.InvokeAsync(() => this.LeftBar.RegisterItems(info));
        
        
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE80F",
            ItemText = Resource.MainPage_Home,
            Tag = "Home",
            IsSelected = true,
            PageType = typeof(MainHomePage)
        });
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE896",
            ItemText = Resource.MainPage_Download,
            Tag = "Download",
            PageType = typeof(MainDownloadPage)
        });
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE716",
            ItemText = Resource.MainPage_Account,
            Tag = "Account",
            PageType = typeof(MainAccountPage)
        });
        if (OperatingSystem.IsWindows())
        {
            RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
            {
                ItemGlyph = "\uF0B9",
                ItemText = "多人联机",
                Tag = "Online",
                PageType = typeof(MainOnlinePage)
            });
        }
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE713",
            ItemText = Resource.MainPage_Setting,
            Tag = "Setting",
            PageType = typeof(MainSettingPage)
        });

        Task.Run(() => LoadPlugins.LoadAll());
    }

    private void TaskBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _ = GlobalModels.TaskPanel.ToggleOpen();
    }
}