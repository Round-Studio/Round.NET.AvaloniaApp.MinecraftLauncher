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
using RMCL.Base.Entry.Style;
using RMCL.Base.Enum;
using RMCL.Base.Enum.BackCall;
using RMCL.Base.Enum.ButtonStyle;
using RMCL.Base.Enum.Update;
using RMCL.Config;
using RMCL.Core.Models.Classes.Manager.StyleManager;
using RMCL.Core.Models.Classes.Manager.TaskManager;
using RMCL.Core.Models.Classes;
using RMCL.Core.Models.Classes.Manager.BackCallManager;
using RMCL.Core.Views.Windows.Initialize;
using RMCL.PathsDictionary;
using RMCL.Plug;
using RMCL.Update;

namespace RMCL.Core.Views;

public partial class MainWindow : Window
{
    private string HomeButtonText;
    public MainWindow()
    {
        Console.WriteLine("Opening MainWindow...");
        UpdateRending();
        Models.Classes.Core.MainWindow = this;
        InitializeComponent();
        
        this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        this.AddHandler(KeyUpEvent, OnKeyUp, RoutingStrategies.Tunnel);
        
        Deactivated += OnWindowDeactivated;
        
        // 订阅鼠标滚轮事件
        this.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
        
        VersionBox.Text = VersionBox.Text.Replace("Version", Assembly.GetEntryAssembly()?.GetName().Version.ToString());
        Models.Classes.Core.TaskView = this.TaskView;
        Models.Classes.Core.MessageShowBox = this.MessageShowBox;
        
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
        UpdateButtonStyle();

        BackCallManager.RegisteredBackCall(() =>
        {
            LoadPlugs.LoadPlugins(PathDictionary.PluginFileFolder);
        }, BackCallType.LaunchedApp);

        BackCallManager.RegisteredBackCall(() =>
        {
            Console.WriteLine("启动游戏...");
        }, BackCallType.LaunchedGame);

        this.Loaded += (s, e) =>
        {
            BackCallManager.Call(BackCallType.LaunchedApp);
        };

        BackCallManager.RegisteredBackCall(() =>
        {
            RequestedThemeVariant = Config.Config.MainConfig.ThemeColors.Theme switch
            {
                ThemeType.Dark => ThemeVariant.Dark,
                ThemeType.Light => ThemeVariant.Light
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
        }, BackCallType.UpdateBackground);
        
        Core.Models.Classes.Core.MainWindow.Topmost = Config.Config.MainConfig.LauncherWindowTopMost;
        
        Models.Classes.Core.Music.Volume = (float)Math.Clamp(Config.Config.MainConfig.BackMusicEntry.Volume, 0.0, 1.0);
        if (Config.Config.MainConfig.BackMusicEntry.Enabled)
        {
            if (!String.IsNullOrWhiteSpace(Config.Config.MainConfig.BackMusicEntry.Path) &&
                Config.Config.MainConfig.Background.ChooseModel != BackgroundModelEnum.Pack) 
            {
                Models.Classes.Core.Music.Enabled = true;
                Models.Classes.Core.Music.Play(Config.Config.MainConfig.BackMusicEntry.Path);
            }
        }
        
        MusicCapsule.SetVolume(Config.Config.MainConfig.BackMusicEntry.Volume * 100);

        // 添加拖放事件处理器
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        // 检查拖拽的数据是否包含文件
        if (e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles();
            if (files != null)
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"拖放文件: {file.Path.LocalPath}");
                    // 在这里处理拖放的文件
                    // 例如：检查文件类型，处理.jar文件等
                }
            }
        }
        e.Handled = true;
    }
    private bool _ctrlPressed = false;
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        {
            _ctrlPressed = true;
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        {
            _ctrlPressed = false;
        }
    }
    
    private void OnWindowDeactivated(object sender, EventArgs e)
    {
        // 窗口失去焦点时重置 Ctrl 键状态
        _ctrlPressed = false;
    }

    private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        if (_ctrlPressed)
        {
            // 根据滚轮方向调整音量
            double delta = e.Delta.Y; // 上滚为正，下滚为负
            double step = 0.01; // 每次滚动的音量变化步长
            
            // 计算新音量
            double newVolume =  Models.Classes.Core.Music.Volume + (delta > 0 ? step : -step);
            if (newVolume * 100 < 0)
            {
                newVolume = 0;  
            }else if (newVolume * 100 > 100)
            {
                newVolume = 1;
            }
            MusicCapsule.SetVolume(newVolume * 100);
            
            Console.WriteLine($"当前音量：{(int)(newVolume * 100)}%");
            
            // 应用新音量
            Models.Classes.Core.Music.Volume = (float)Math.Clamp(newVolume, 0.0, 1.0);
            
            Config.Config.MainConfig.BackMusicEntry.Volume = newVolume;
            Config.Config.SaveConfig();
            
            // 阻止事件继续冒泡
            e.Handled = true;
        }
    }
    
    public void UpdateButtonStyle()
    {
        if (Config.Config.MainConfig.ButtonStyle.HomeButton == OrdinaryButtonStyle.Default)
        {
            HomeButtonText = "Round Minecraft Launcher";
        }
        else
        {
            HomeButtonText = "RMCL";
        }
        
        HomeButton.Content = HomeButtonText;
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

    private void HomeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (HomeButton.Content.ToString() != HomeButtonText)
        {
            Models.Classes.Core.ChildFrame.Close();
        }
        else
        {
            Models.Classes.Core.BottomBar.NavigationTo("Home");
        }
    }

    private void TaskButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Models.Classes.Core.TaskView.Show();
    }

    public void UpdateRending()
    {
        RenderOptions.SetTextRenderingMode(this, Config.Config.MainConfig.RenderModel.TextRenderingMode); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, Config.Config.MainConfig.RenderModel.BitmapInterpolationMode); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, Config.Config.MainConfig.RenderModel.EdgeMode); // 形状渲染模式
    }
}