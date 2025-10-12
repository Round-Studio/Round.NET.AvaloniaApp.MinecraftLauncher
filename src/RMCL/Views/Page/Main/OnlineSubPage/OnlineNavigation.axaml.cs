using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RemoteOnline.Core;
using RMCL.Models.Global;
using RMCL.Views.Page.DialogContent;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.OnlineSubPage;

public partial class OnlineNavigation : UserControl
{
    public OnlineNavigation()
    {
        InitializeComponent();
    }

    private void Host_OnClick(object? sender, RoutedEventArgs e)
    {
        MainOnlinePage.NavigationFrame.NavigateTo(new OnlineHostChooseGame());
    }

    private void Link_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new DialogInputOnlineCodeContent();
        DialogHost.Show(new DialogInfo()
        {
            Title = "输入联机码",
            CloseButtonText = "加入",
            CloseAction = () =>
            {
                if(string.IsNullOrEmpty(dialog.Code)) return;
                
                OnlineLink.OnlineService = new OnlineService(Path.Combine(PathsList.OnlinePath, "easytier-core.exe"));
                OnlineLink.OnlineService.LinkRoom(dialog.Code); 
                MainOnlinePage.NavigationFrame.NavigateTo(new OnlineLink(OnlineLink.OnlineService));
            },
            PrimaryButtonText = "取消",
            AccountButton = DialogButtons.CloseButton,
            Content = dialog
        });
    }
}