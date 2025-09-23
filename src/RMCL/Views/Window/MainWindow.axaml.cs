using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using RMCL.Models.Global;
using RMCL.Views.Page.Main;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式

        GlobalModels.NoticePanel = NoticePanel;
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
        GlobalModels.MainPageContent.Content = new MainHomePage();
    }
}