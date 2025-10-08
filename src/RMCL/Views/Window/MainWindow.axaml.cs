using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Style.Core;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Base.Entry.Config;
using RMCL.Base.Entry.Notice;
using RMCL.Base.Enum.Notice;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Models.Plugin;
using RMCL.Views.Control.Notice;
using RMCL.Views.Control.Tasks;
using RMCL.Views.Page.Main;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Logger;
using Round.SDK.Plugin;
using Round.SDK.Plugin.RMCL;

namespace RMCL.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ConsoleRedirector.RegisterThread(Thread.CurrentThread,"MainWindow");
        Console.WriteLine("载入 MainWindow");
        GlobalModels.MainWindow = this;
        
        InitializeComponent();
        Console.WriteLine("窗体初始化完成");

        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        Console.WriteLine("渲染模式设置完毕");

        GlobalModels.NoticePanel = NoticePanel;
        GlobalModels.TaskPanel = TaskPanel;

        if (TaskPanel.GetOpenState()) TaskPanel.ToggleOpen();

        ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.StyleConfig.LightThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        UpdateBack();
        Console.WriteLine("主题设置完毕");

        if (GlobalModels.Config.Data.WindowInfo.X != -1 && GlobalModels.Config.Data.WindowInfo.Y != -1)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(x: GlobalModels.Config.Data.WindowInfo.X,
                y: GlobalModels.Config.Data.WindowInfo.Y);

            this.Width = GlobalModels.Config.Data.WindowInfo.Width;
            this.Height = GlobalModels.Config.Data.WindowInfo.Height;
        }
        Console.WriteLine("窗体位置信息初始完毕");
    }

    public async Task UpdateBack()
    {
        # region 更新材质
        var name = BackMaterialHelper.GetStringName(GlobalModels.Config.Data.StyleConfig.BackMaterialType);
        if (!string.IsNullOrEmpty(name))
        {
            var uri = new Uri($"avares://RMCL/Assets/Image/{name}");

            // 2. 使用 AssetLoader.Open 获取流
            using (var stream = AssetLoader.Open(uri))
            {
                // 3. 将流解码为 Bitmap
                var bitmap = new Bitmap(stream);
    
                // 4. 现在你可以将 bitmap 赋值给 Image 控件的 Source 属性
                RightImage.Opacity = 0;
                LeftImage.Opacity = 0;

                await Task.Delay(180);
            
                RightImage.Source = bitmap;
                LeftImage.Source = bitmap;
            
                RightImage.Opacity = 0.9;
                LeftImage.Opacity = 0.9;
            }
        }
        else
        {
            RightImage.Opacity = 0;
            LeftImage.Opacity = 0;
        }
        # endregion

        #region 更新背景

        this.TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.Transparent };
        BackgroundBox.IsVisible = false;
        AccentBackgroundBox.IsVisible = false;
        if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Mica)
        {
            this.TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.Mica };
        }else if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Blur)
        {
            this.TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.AcrylicBlur };
        }else if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Image)
        {
            BackgroundImageOpacity.Opacity = (100 - GlobalModels.Config.Data.StyleConfig.BackgroundImageOpacity) * 0.01;
            
            var index = GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex;
            if (index != -1)
            {
                if (GlobalModels.Config.Data.StyleConfig.BackgroundImages.Count >= 0)
                {
                    BackgroundBox.IsVisible = true;
                    BackgroundBox.Effect = new BlurEffect()
                    {
                        Radius = GlobalModels.Config.Data.StyleConfig.BackgroundImageBlur
                    };
                    BackgroundBox.Margin = new Thickness(-GlobalModels.Config.Data.StyleConfig.BackgroundImageBlur);
                    
                    BackgroundImage.Background = new ImageBrush()
                    {
                        Stretch = Stretch.UniformToFill,
                        Source = new Bitmap(
                            GlobalModels.Config.Data.StyleConfig.BackgroundImages[
                                GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex])
                    };
                }
            }

        }else if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.AccentColor)
        {
            AccentBackgroundBox.IsVisible = true;
        }

        #endregion
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    private void MinBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState == WindowState.Maximized ?  WindowState.Normal : WindowState.Maximized;
    }

    private void CloseBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
        
        Environment.Exit(0);
    }

    private void TitleBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TaskPanel.GetOpenState())
        {
            TaskPanel.ToggleOpen();
        }else GlobalModels.MainPageContent.NavigateTo(new MainHomePage());
    }

    private void TopLevel_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        GlobalModels.Config.Data.WindowInfo = new WindowPoint()
        {
            Width = this.Bounds.Width,
            Height = this.Bounds.Height,
            X = this.Position.X,
            Y = this.Position.Y
        };
        
        GlobalModels.Config.Save();
    }
}