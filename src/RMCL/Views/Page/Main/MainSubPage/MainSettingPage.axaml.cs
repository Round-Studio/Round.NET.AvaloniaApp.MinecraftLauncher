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
using RMCL.Base.Entry.Notice;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using RMCL.Views.Page.Main.SettingsSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainSettingPage : UserControl
{
    public static MainSettingPage Page { get; set; }
    public static object BackPage { get; set; } = new SettingsNavigation();

    public MainSettingPage()
    {
        InitializeComponent();
        BackPage = new SettingsNavigation();
        Page = this;

        SettingsNavigation.NavigateTo(new SettingsNavigation());

        BreadcrumbBar.RootItemClick = () => NavigationTo(new SettingsNavigation());
        TripVersion.Text = $"{Assembly.GetEntryAssembly().GetName().Version}";
        TripBuildDate.Text = $"{CheckVersion.GetLinkerTimestamp()}";
    }

    public void SetReStart()
    {
        RestartBtn.IsVisible = true;
        
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
        // 获取当前应用程序的路径和文件名
        string applicationPath = Process.GetCurrentProcess().MainModule.FileName;
            
        // 启动新的应用程序实例
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = applicationPath,
            UseShellExecute = true
        };
            
        // 启动新实例
        Process.Start(startInfo);
            
        // 关闭当前应用程序
        Environment.Exit(0);
    }
}