using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plugin.MusicPlayer.Views.Windows;

public partial class CapsuleWindow : Window
{
    public CapsuleWindow()
    {
        InitializeComponent();
        
        Opened += (sender, args) => PositionWindow();
    }
    private void PositionWindow()
    {
        // 获取主屏幕信息
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        Position = new PixelPoint(
            (workingArea.Width - (int)Width) / 2, // 水平居中
            0
        );
    }
}