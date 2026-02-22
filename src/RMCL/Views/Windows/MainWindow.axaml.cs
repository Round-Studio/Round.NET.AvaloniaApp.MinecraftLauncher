using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using RMCL.Base.Entry.Config;
using RMCL.Interface;
using RMCL.Models.Global;
using RMCL.Views.Pages.MainPages;

namespace RMCL.Views.Windows;

public partial class MainWindow : Window
{
    private const string MaximizeGlyph = "\uE922";   // 最大化图标
    private const string RestoreGlyph = "\uE923";    // 还原图标
    
    public MainWindow()
    {
        Console.WriteLine(@"载入 MainWindow");
        GlobalModels.MainWindow = this;

        InitializeComponent();
        Console.WriteLine(@"窗体初始化完成");

        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        Console.WriteLine(@"渲染模式设置完毕");

        if (GlobalModels.Config.Data.WindowInfo.X != -1 && GlobalModels.Config.Data.WindowInfo.Y != -1)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Position = new PixelPoint(GlobalModels.Config.Data.WindowInfo.X,
                GlobalModels.Config.Data.WindowInfo.Y);

            Width = GlobalModels.Config.Data.WindowInfo.Width;
            Height = GlobalModels.Config.Data.WindowInfo.Height;

            Console.WriteLine(
                $@"Main Window: Width {GlobalModels.Config.Data.WindowInfo.Width}, Height {GlobalModels.Config.Data.WindowInfo.Height}");
        }

        Console.WriteLine(@"窗体位置信息初始完毕");

        NavigateTo(new HomePage());

        Task.Run(() =>
        {
            while (true)
            {
                Dispatcher.UIThread.Invoke(() => OnWindowStateChanged(this.WindowState));
                Thread.Sleep(500);
            }
        });
    }

    public void NavigateTo(IPage page)
    {
        TopNavBar.NavigateTo(page.TopNavBar);
        MainFrame.NavigateTo(page);
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
    
    private void TopLevel_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        GlobalModels.Config.Data.WindowInfo = new WindowPoint
        {
            Width = Bounds.Width,
            Height = Bounds.Height,
            X = Position.X,
            Y = Position.Y
        };

        GlobalModels.Config.Save();
    }
    
    #region 窗口控制按钮事件
    
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void MaximizeRestoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        // 如果窗口处于最小化状态，点击后恢复到正常状态
        else if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
    }
    
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void OnWindowStateChanged(WindowState state)
    {
        this.Padding = new Thickness(0);
        if (MaximizeRestoreIcon != null)
        {
            switch (state)
            {
                case WindowState.Normal:
                    MaximizeRestoreIcon.Glyph = MaximizeGlyph;
                    break;
                
                case WindowState.Maximized:
                    MaximizeRestoreIcon.Glyph = RestoreGlyph;
                    if (OperatingSystem.IsWindows())
                        this.Padding = new Thickness(8);
                    break;
                
                case WindowState.Minimized:
                    MaximizeRestoreIcon.Glyph = MaximizeGlyph;
                    break;
            }
        }
    }
    
    #endregion
}