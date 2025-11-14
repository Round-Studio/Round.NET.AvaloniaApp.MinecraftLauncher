using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using RMCL.Views.Page.Main.SettingsSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainSettingPage : UserControl
{
    public static MainSettingPage Page { get; set; }
    public static object BackPage { get; set; } = new SettingsNavigation();
    private static bool IsReStart = false;

    public MainSettingPage()
    {
        InitializeComponent();
        BackPage = new SettingsNavigation();

        if (IsReStart)
        {
            RestartBtn.IsVisible = true;
        }
        
        Page = this;

        SettingsNavigation.NavigateTo(new SettingsNavigation());

        BreadcrumbBar.RootItemClick = () => NavigationTo(new SettingsNavigation());
        TripVersion.Text = $"v{Assembly.GetEntryAssembly().GetName().Version}";
        TripBuildDate.Text = $"Build.{CheckVersion.GetLinkerTimestamp().ToString("yyyy.MM.dd.hhmm")}";
    }

    public void SetReStart()
    {
        RestartBtn.IsVisible = true;
        IsReStart = true; 
        
        GlobalModels.NoticePanel.AddNotice(new NoticeInfo()
        {
            Message = "当前设置需要重启启动器才能完全生效。",
            Title = "需要重启"
        });
    }

    public async Task NavigationTo(object obj)
    {
        SettingsNavigation.NavigateTo(obj);
        if (obj is SettingsNavigation)
        {
            // BackBtn.IsEnabled = false;
            BackBtn.Margin = new Thickness(-80, 23,80,23);
            await Task.Delay(100);
            BreadcrumbBar.Margin = new Thickness(20);
            BreadcrumbBar.SetItems(new ());
        }
        else
        {
            // BackBtn.IsEnabled = true;
            BreadcrumbBar.Margin = new Thickness(64,20,0,20);
            await Task.Delay(100);
            BackBtn.Margin = new Thickness(20, 23);
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigationTo(Activator.CreateInstance(BackPage.GetType()));
    }

    private void RestartBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainView.Instance.GoToMainPage();
    }
}