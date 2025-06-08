using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Base.Enum.Update;
using RMCL.Models.Classes;

namespace RMCL.Views.Pages.Main.SettingPages;

public partial class DownloadSetting : UserControl
{
    public bool IsEdit { get; set; } = false;
    public DownloadSetting()
    {
        InitializeComponent();

        ChooseDownloadSource.SelectedIndex = Config.Config.MainConfig.UpdateModel.Proxy.GetHashCode();
        ChoosePublishSource.SelectedIndex = Config.Config.MainConfig.UpdateModel.Branch.GetHashCode();
        ChooseUpdateAPISource.SelectedIndex = Config.Config.MainConfig.UpdateModel.Route.GetHashCode();
        
        IsEdit = true;
    }

    private void ChoosePublishSource_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            Config.Config.MainConfig.UpdateModel.Branch = (UpdateBranch)ChoosePublishSource.SelectedIndex;
            Config.Config.SaveConfig();
        }
    }

    private void ChooseDownloadSource_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            Config.Config.MainConfig.UpdateModel.Proxy = (UpdateProxy)ChooseDownloadSource.SelectedIndex;
            Config.Config.SaveConfig();
        }
    }

    private void ChooseUpdateAPISource_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
        if (IsEdit)
        {
            Config.Config.MainConfig.UpdateModel.Route = (UpdateRoute)ChooseUpdateAPISource.SelectedIndex;
            Config.Config.SaveConfig();
        }
    }
}