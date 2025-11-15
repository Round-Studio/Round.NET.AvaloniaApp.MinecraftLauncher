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
using Avalonia.Platform;
using Avalonia.Styling;
using OnePointUI.Avalonia.Style.Core;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.WindowFrame;
using RMCL.Base.Entry.Config;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Views.Page.Main.DragDropPage;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Logger;

namespace RMCL.Views;

public partial class MainWindow : OnePointWindow
{
    public MainWindow()
    {
        ConsoleRedirector.RegisterThread(Thread.CurrentThread,"MainWindow");
        Console.WriteLine(@"载入 MainWindow");
        GlobalModels.MainWindow = this;
        
        InitializeComponent();
        Console.WriteLine(@"窗体初始化完成");

        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        Console.WriteLine(@"渲染模式设置完毕");

        GlobalModels.NoticePanel = OnePointUI.Avalonia.Styling.Controls.OnePointControls.Notice.Info.NoticePanel.InstancePanel;
        GlobalModels.TaskPanel = TaskPanel;

        if (TaskPanel.GetOpenState()) TaskPanel.ToggleOpen();

        ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.StyleConfig.LightThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        UpdateBack();
        Console.WriteLine(@"主题设置完毕");

        if (GlobalModels.Config.Data.WindowInfo.X != -1 && GlobalModels.Config.Data.WindowInfo.Y != -1)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(x: GlobalModels.Config.Data.WindowInfo.X,
                y: GlobalModels.Config.Data.WindowInfo.Y);

            this.Width = GlobalModels.Config.Data.WindowInfo.Width;
            this.Height = GlobalModels.Config.Data.WindowInfo.Height;

            Console.WriteLine(
                $@"Main Window: Width {GlobalModels.Config.Data.WindowInfo.Width}, Height {GlobalModels.Config.Data.WindowInfo.Height}");
        }

        Console.WriteLine(@"窗体位置信息初始完毕");
        
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        /*if (e.Data.Contains(DataFormats.FileNames))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }*/
        
        e.DragEffects = DragDropEffects.Copy;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames()?.ToList();
            if (files != null && files.Any())
            {
                files.ForEach(x => Console.WriteLine($"检测到拖拽文件：{x}"));
                this.OpenDraw(new DragDropRoot(files),"拖拽文件");
            }
        }
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
                    SetBackgroundBlur(GlobalModels.Config.Data.StyleConfig.BackgroundImageBlur);
                    
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

    public void SetBackgroundBlur(int num)
    {
        BackgroundBox.Effect = new BlurEffect()
        {
            Radius = num
        };
        BackgroundBox.Margin = new Thickness(-num);
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