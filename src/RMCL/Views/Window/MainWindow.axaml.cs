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
using RMCL.Models;
using RMCL.Models.Global;
using Round.SDK.Logger;

namespace RMCL.Views;

public partial class MainWindow : OnePointWindow
{
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
            }
        }
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