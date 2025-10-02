using System;
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
using RMCL.Views.Page.Main;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        GlobalModels.MainWindow = this;
        
        InitializeComponent();

        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式

        GlobalModels.NoticePanel = NoticePanel;
        GlobalModels.TaskPanel = TaskPanel;
        
        /*GlobalModels.NoticePanel.AddNotice(new NoticeInfo()
        {
            Message = "欢迎使用 RMCL",
            Title = "Welcome to RMCL",
            NoticeType = NoticeType.Info
        });*/

        if (TaskPanel.GetOpenState()) TaskPanel.ToggleOpen();

        ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.ThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        UpdateBack();

        if (GlobalModels.Config.Data.WindowInfo.X != -1 && GlobalModels.Config.Data.WindowInfo.Y != -1)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(x: GlobalModels.Config.Data.WindowInfo.X,
                y: GlobalModels.Config.Data.WindowInfo.Y);

            this.Width = GlobalModels.Config.Data.WindowInfo.Width;
            this.Height = GlobalModels.Config.Data.WindowInfo.Height;
        }
    }

    public async Task UpdateBack()
    {
        var uri = new Uri($"avares://RMCL/Assets/Image/{BackMaterialHelper.GetStringName(GlobalModels.Config.Data.BackMaterialType)}");

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
            
            RightImage.Opacity = 0.3;
            LeftImage.Opacity = 0.3;
        }
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

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        /*DialogHost.Show(new DialogInfo()
        {
            Title = "预览版警告",
            Content = "当前版本仅为 RMCL 4 预览版，\n" +
                      "仅作为 UI/UX 测试发布。\n" +
                      "请勿将此版本的 RMCL 加入整合包内发布！",
            CloseButtonText = "我知道了",
            AccountButton = DialogButtons.CloseButton
        });*/
    }
}