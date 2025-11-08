using Avalonia.Threading;
using Plugin.MusicPlayer.Views.Pages;
using Plugin.MusicPlayer.Views.Windows;
using Round.SDK.Entry.RMCL;
using Round.SDK.Plugin.RMCL;
using Round.SDK.Plugin.RMCL.Register;

namespace Plugin.MusicPlayer;

public class Plugin : IPluginRMCL
{
    public void Initialize()
    {
        RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
        {
            ItemGlyph = "\uE8D6",
            Tag = "MusicPlayer",
            ItemText = "音乐",
            PageType = typeof(MainHomePage)
        });

        Dispatcher.UIThread.Invoke(() =>
        {
            var window = new CapsuleWindow();
            window.Show();
        });
    }
}