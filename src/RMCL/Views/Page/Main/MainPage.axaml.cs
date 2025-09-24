using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Base.Entry.Navigation;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main;

public partial class MainPage : UserControl
{
    private string oldTag = "Home";
    public MainPage()
    {
        InitializeComponent();

        GlobalModels.MainPageContent = this.MainPageContent;
        MainPageContent.NavigateTo(new MainHomePage());
        
        BottomBar.OnNavigation = tag =>
        {
            if (oldTag != tag)
            {
                oldTag = tag;
                switch (tag)
                {
                    case "Home":
                        MainPageContent.NavigateTo(new MainHomePage());
                        break;
                    case "Download":
                        MainPageContent.NavigateTo(new MainDownloadPage());
                        break;
                    case "Setting":
                        MainPageContent.NavigateTo(new MainSettingPage());
                        break;
                }
            }
        };
        
        BottomBar.RegisterItems(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE713",
            ItemText = Resource.MainPage_Setting,
            Tag = "Setting"
        });
        BottomBar.RegisterItems(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE896",
            ItemText = Resource.MainPage_Download,
            Tag = "Download"
        });
        BottomBar.RegisterItems(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE80F",
            ItemText = Resource.MainPage_Home,
            Tag = "Home",
            IsSelected = true
        });
    }
}