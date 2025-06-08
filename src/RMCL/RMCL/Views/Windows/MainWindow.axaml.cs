using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Avalonia.Styling;
using Avalonia.Threading;
using FluentAvalonia.Interop;
using FluentAvalonia.UI.Controls;
using RMCL.Base.Enum.Update;
using RMCL.Config;
using RMCL.Models.Classes;
using RMCL.Models.Classes.Manager.StyleManager;
using RMCL.Models.Classes.Manager.TaskManager;
using RMCL.Update;

namespace RMCL.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Console.WriteLine("Opening MainWindow...");
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        
        Core.MainWindow = this;
        InitializeComponent();
        VersionBox.Text = VersionBox.Text.Replace("Version", Assembly.GetEntryAssembly()?.GetName().Version.ToString());
        Core.TaskView = this.TaskView;
        
        this.Closing += (e,sender) => {
            Config.Config.MainConfig.WindowWidth = (int)this.Bounds.Width;
            Config.Config.MainConfig.WindowHeight = (int)this.Bounds.Height;
            Config.Config.MainConfig.WindowX = (int)this.Position.X;
            Config.Config.MainConfig.WindowY = (int)this.Position.Y;
            Config.Config.MainConfig.FirstLauncher = false;
            Config.Config.SaveConfig();
        };

        this.Width = Config.Config.MainConfig.WindowWidth;
        this.Height = Config.Config.MainConfig.WindowHeight;
        if (Config.Config.MainConfig.WindowX != 0 && Config.Config.MainConfig.WindowY != 0)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(Config.Config.MainConfig.WindowX, Config.Config.MainConfig.WindowY);
        }

        HomeButtonTips.IsOpen = Config.Config.MainConfig.FirstLauncher;

        StyleManager.UpdateBackground();
        
        Console.WriteLine("Opened MainWindow!");
    }

    public void UpdateStatus(int num)
    {
        TaskButton.UpdateTaskStatus(num);
    }
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        var brush = DisplayPath.Fill;
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        var btn = (Button)sender;
        btn.Content = WindowState == WindowState.Maximized
            ? new PathIcon()
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(0, 3, 0, 0),
                Foreground = brush,
                Data = StreamGeometry.Parse(
                    "M832 704H704v128c0 70.692-57.308 128-128 128H192c-70.692 0-128-57.308-128-128V448c0-70.692 57.308-128 128-128h128V192c0-70.692 57.308-128 128-128h384c70.692 0 128 57.308 128 128v384c0 70.692-57.308 128-128 128zM192 384c-35.346 0-64 28.654-64 64v384c0 35.346 28.654 64 64 64h384c35.346 0 64-28.654 64-64V448c0-35.346-28.654-64-64-64H192z m704-192c0-35.346-28.654-64-64-64H448c-35.346 0-64 28.654-64 64v128h192c70.692 0 128 57.308 128 128v192h128c35.346 0 64-28.654 64-64V192z")
            }
            : new PathIcon()
            {
                Margin = new Thickness(0, 1, 0, 0),
                Foreground = brush,
                Width = 12,
                Height = 12,
                Data = StreamGeometry.Parse(
                    "M233.301333 128A105.301333 105.301333 0 0 0 128 233.301333v557.397334A105.301333 105.301333 0 0 0 233.301333 896h557.397334A105.301333 105.301333 0 0 0 896 790.698667V233.301333A105.301333 105.301333 0 0 0 790.698667 128H233.301333z m-18.602666 105.301333c0-10.24 8.32-18.602667 18.602666-18.602666h557.397334c10.24 0 18.602667 8.32 18.602666 18.602666v557.397334c0 10.24-8.32 18.602667-18.602666 18.602666H233.301333a18.56 18.56 0 0 1-18.602666-18.602666V233.301333z")
            };
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (TaskManager.TaskList.Count != 0)
        {
            new CloseWindow().ShowDialog(this);
        }
        else Close();
    }

    public void ChangeMenuItems(List<MenuItem> menuItems)
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].CornerRadius = new CornerRadius(12);
        }
        Menu.Margin = new Thickness(20, 0, -20, 0);
        Menu.Opacity = 0;
        Task.Run(() =>
        {
            Thread.Sleep(120);
            Dispatcher.UIThread.Invoke((() =>Menu.Margin = new Thickness(-20, 0, 20, 0)));
            Thread.Sleep(150);
            Dispatcher.UIThread.Invoke(() =>
            {
                Menu.ItemsSource = menuItems;
                Menu.Margin = new Thickness(0, 0, 0, 0);
                Menu.Opacity = 1;
            });
        });
    }
    // 标题栏拖拽逻辑
    private void TitleBar_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        var brush = DisplayPath.Fill;
        var btn = Maxbtn;
        btn.Content = WindowState == WindowState.Maximized
            ? new PathIcon()
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(0, 3, 0, 0),
                Foreground = brush,
                Data = StreamGeometry.Parse(
                    "M832 704H704v128c0 70.692-57.308 128-128 128H192c-70.692 0-128-57.308-128-128V448c0-70.692 57.308-128 128-128h128V192c0-70.692 57.308-128 128-128h384c70.692 0 128 57.308 128 128v384c0 70.692-57.308 128-128 128zM192 384c-35.346 0-64 28.654-64 64v384c0 35.346 28.654 64 64 64h384c35.346 0 64-28.654 64-64V448c0-35.346-28.654-64-64-64H192z m704-192c0-35.346-28.654-64-64-64H448c-35.346 0-64 28.654-64 64v128h192c70.692 0 128 57.308 128 128v192h128c35.346 0 64-28.654 64-64V192z")
            }
            : new PathIcon()
            {
                Margin = new Thickness(0, 1, 0, 0),
                Foreground = brush,
                Width = 12,
                Height = 12,
                Data = StreamGeometry.Parse(
                    "M233.301333 128A105.301333 105.301333 0 0 0 128 233.301333v557.397334A105.301333 105.301333 0 0 0 233.301333 896h557.397334A105.301333 105.301333 0 0 0 896 790.698667V233.301333A105.301333 105.301333 0 0 0 790.698667 128H233.301333z m-18.602666 105.301333c0-10.24 8.32-18.602667 18.602666-18.602666h557.397334c10.24 0 18.602667 8.32 18.602666 18.602666v557.397334c0 10.24-8.32 18.602667-18.602666 18.602666H233.301333a18.56 18.56 0 0 1-18.602666-18.602666V233.301333z")
            };
    }

    private void HomeButtonTips_OnCloseButtonClick(TeachingTip sender, EventArgs args)
    {
        Config.Config.MainConfig.FirstLauncher = false;
        Config.Config.SaveConfig();
    }

    private void DisplayModeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Config.Config.MainConfig.Theme = Config.Config.MainConfig.Theme switch
        {
            ThemeType.Light => ThemeType.Dark,
            ThemeType.Dark => ThemeType.Light
        };
        Config.Config.SaveConfig();
        
        
        RequestedThemeVariant = Config.Config.MainConfig.Theme switch
        {
            ThemeType.Dark => ThemeVariant.Dark,
            ThemeType.Light => ThemeVariant.Light,
        };
        
        var brush = DisplayPath.Fill;
        var btn = Maxbtn;
        btn.Content = WindowState == WindowState.Maximized
            ? new PathIcon()
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(0, 3, 0, 0),
                Foreground = brush,
                Data = StreamGeometry.Parse(
                    "M832 704H704v128c0 70.692-57.308 128-128 128H192c-70.692 0-128-57.308-128-128V448c0-70.692 57.308-128 128-128h128V192c0-70.692 57.308-128 128-128h384c70.692 0 128 57.308 128 128v384c0 70.692-57.308 128-128 128zM192 384c-35.346 0-64 28.654-64 64v384c0 35.346 28.654 64 64 64h384c35.346 0 64-28.654 64-64V448c0-35.346-28.654-64-64-64H192z m704-192c0-35.346-28.654-64-64-64H448c-35.346 0-64 28.654-64 64v128h192c70.692 0 128 57.308 128 128v192h128c35.346 0 64-28.654 64-64V192z")
            }
            : new PathIcon()
            {
                Margin = new Thickness(0, 1, 0, 0),
                Foreground = brush,
                Width = 12,
                Height = 12,
                Data = StreamGeometry.Parse(
                    "M233.301333 128A105.301333 105.301333 0 0 0 128 233.301333v557.397334A105.301333 105.301333 0 0 0 233.301333 896h557.397334A105.301333 105.301333 0 0 0 896 790.698667V233.301333A105.301333 105.301333 0 0 0 790.698667 128H233.301333z m-18.602666 105.301333c0-10.24 8.32-18.602667 18.602666-18.602666h557.397334c10.24 0 18.602667 8.32 18.602666 18.602666v557.397334c0 10.24-8.32 18.602667-18.602666 18.602666H233.301333a18.56 18.56 0 0 1-18.602666-18.602666V233.301333z")
            };
        
        StyleManager.UpdateBackground();
    }

    private void HomeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (HomeButton.Content.ToString() != "Round Minecraft Launcher")
        {
            Core.ChildFrame.Close();
        }
        else
        {
            Core.BottomBar.NavigationTo("Home");
        }
    }

    private void TaskButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Core.TaskView.Show();
    }
}