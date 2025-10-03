using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Round.SDK.Entry.RMCL;
using RMCL.Models.Global;
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

        BottomBar.OnNavigation = tag =>
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            object? page = Activator.CreateInstance(tag);
            if(page == null) throw new NullReferenceException(nameof(page));
            
            MainPageContent.NavigateTo(page);
        };

        RegisterService.API.RegisterBottomBarItem = info => BottomBar.RegisterItems(info);
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE713",
            ItemText = Resource.MainPage_Setting,
            Tag = "Setting",
            PageType = typeof(MainSettingPage)
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
            ItemGlyph = "\uE80F",
            ItemText = Resource.MainPage_Home,
            Tag = "Home",
            IsSelected = true,
            PageType = typeof(MainHomePage)
        });
    }

    private void TaskBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _ = GlobalModels.TaskPanel.ToggleOpen();
    }
}