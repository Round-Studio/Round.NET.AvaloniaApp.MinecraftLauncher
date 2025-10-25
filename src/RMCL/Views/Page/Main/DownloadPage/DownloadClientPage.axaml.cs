using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OverrideLauncher.Core.Base.Entry.Download.Install.Manifest;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.DownloadPage;

public partial class DownloadClientPage : UserControl
{
    public ManifestMojang.ManifestVersion ManifestVersion;
    public DownloadClientPage()
    {
        InitializeComponent();
    }

    public void UpdateUI()
    {
        MainDownloadPage.Instance.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = $"{ManifestVersion.Id}",
                ItemClickAction = (s) => { }
            }
        });

        ClientName.Text = ManifestVersion.Id;
        ClientName.Watermark = ManifestVersion.Id;

        if (ManifestVersion.Type == "snapshot")
        {
            OutherInstallItem.IsVisible = false;
        }
    }
    
    public DownloadClientPage(ManifestMojang.ManifestVersion _m) : this()
    {
        ManifestVersion = _m;
        
        UpdateUI();
    }
}