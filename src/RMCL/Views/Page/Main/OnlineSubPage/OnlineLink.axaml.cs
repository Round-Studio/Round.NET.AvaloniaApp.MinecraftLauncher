using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RemoteOnline.Core;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.OnlineSubPage;

public partial class OnlineLink : UserControl
{
    public static OnlineService OnlineService { get; set; } = null;
    public OnlineLink(OnlineService service)
    {
        if(OnlineService == null) OnlineService = service;
        InitializeComponent();
        
        ClientPort.Text = OnlineService.LocalPort.ToString();
        ClientIP.Text = $"127.0.0.1:{OnlineService.LocalPort}";
        ClientCode.Text = OnlineService.OnlineCode;

        Task.Run(OnlineService.Run);
    }

    private void CloseRoom_OnClick(object? sender, RoutedEventArgs e)
    {
        OnlineService.Stop();
        OnlineService = null;
        MainOnlinePage.NavigationFrame.NavigateTo(new OnlineNavigation());
    }
}