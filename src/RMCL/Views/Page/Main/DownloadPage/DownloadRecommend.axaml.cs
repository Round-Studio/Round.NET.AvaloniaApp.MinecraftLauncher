using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
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

    public void UpdateUI()
    {
        Task.Run(() =>
        {
            Console.WriteLine("开始多线程异步加载推荐下载项 01 - 最新版本");

            if(ManifestMojang == null) ManifestMojang = InstallHelper.GetVersionManifest().Result;
            if (ManifestMojang != null)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    VersionRelease.Header = ManifestMojang.Latest.Release;
                    VersionPreview.Header = ManifestMojang.Latest.Snapshot;

                    ResultBox.IsVisible = true;
                    LoadRing.IsVisible = false;
                });
            }
        });
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