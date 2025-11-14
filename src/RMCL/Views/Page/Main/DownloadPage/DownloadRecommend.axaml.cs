using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using OverrideLauncher.Core.Base.Entry.Download.Install.Manifest;
using OverrideLauncher.Core.Classes.Install.Manifest;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.DownloadPage;

public partial class DownloadRecommend : UserControl
{
    public static ManifestMojang? ManifestMojang { get; set; } = null;
    
    public DownloadRecommend()
    {
        InitializeComponent();

        UpdateUI();
    }

    public async Task UpdateUI()
    {
        Console.WriteLine(@"开始多线程异步加载推荐下载项 01 - 最新版本");

        try
        {
            if (ManifestMojang == null) ManifestMojang = await InstallHelper.GetVersionManifest();
            if (ManifestMojang != null)
            {
                VersionRelease.Header = ManifestMojang.Latest.Release;
                VersionPreview.Header = ManifestMojang.Latest.Snapshot;

                ResultBox.IsVisible = true;
                LoadRing.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            DialogHost.Show(new DialogInfo()
            {
                Title = "网络错误",
                Content = $"请检查本机网络是否正常连接。\n\n错误信息：\n{ex}",
                CloseButtonText = "好"
            });
        }
    }

    private ManifestMojang.ManifestVersion? FindVersion(string id)
    {
        return ManifestMojang?.Versions.FindLast(x => x.Id == id);
    }
    
    private void VersionRelease_OnClick(object? sender, RoutedEventArgs e)
    {
        var body = FindVersion(ManifestMojang.Latest.Release);
        
        MainDownloadPage.Instance.DownloadFrame.NavigateTo(new DownloadClientPage(body));
    }

    private void VersionPreview_OnClick(object? sender, RoutedEventArgs e)
    {
        var body = FindVersion(ManifestMojang.Latest.Snapshot);
        
        MainDownloadPage.Instance.DownloadFrame.NavigateTo(new DownloadClientPage(body));
    }
}