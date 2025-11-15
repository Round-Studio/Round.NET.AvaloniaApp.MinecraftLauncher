using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Entry.Helper;
using Round.SDK.Helper.IO;
using Path = System.IO.Path;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorDisk : UserControl
{
    public BehaviorDisk()
    {
        InitializeComponent();
        
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Behavior,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsBehavior());
                }
            },
            new BreadcrumbItemInfo()
            {
                ItemName = "储存感知",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new BehaviorDisk());
                }
            }
        });
        
        // 创建监视器实例，注册总变化回调
        var monitor = new FileStorageMonitor(TotalChangeCallback);
        
        // 添加要监视的文件夹
        monitor.Add(new MonitorEntry
        {
            Path = Path.GetDirectoryName(PathsList.ConfigPath),
            BackCall = (totalSize, currentSize) => 
            {
                Console.WriteLine($"配置文件夹变化 - 当前大小: {FormatSize(currentSize)}, 总大小: {FormatSize(totalSize)}");

                Dispatcher.UIThread.Invoke(() =>
                {
                    ConfigText.Text = $"{FormatSize(currentSize)} / {CalculateThePercentage(totalSize, currentSize):0.00} %";
                    ConfigProgressBar.Value = CalculateThePercentage(totalSize, currentSize);
                });
            }
        });
        
        monitor.Add(new MonitorEntry
        {
            Path = PathsList.PluginPath,
            BackCall = (totalSize, currentSize) =>
            {
                Console.WriteLine($"插件文件夹变化 - 当前大小: {FormatSize(currentSize)}, 总大小: {FormatSize(totalSize)}");

                Dispatcher.UIThread.Invoke(() =>
                {
                    PacketText.Text = $"{FormatSize(currentSize)} / {CalculateThePercentage(totalSize, currentSize):0.00} %";
                    PacketProgressBar.Value = CalculateThePercentage(totalSize, currentSize);
                });
            }
        });
        
        monitor.Add(new MonitorEntry
        {
            Path = PathsList.TempPath,
            BackCall = (totalSize, currentSize) =>
            {
                Console.WriteLine($"临时文件夹变化 - 当前大小: {FormatSize(currentSize)}, 总大小: {FormatSize(totalSize)}");

                Dispatcher.UIThread.Invoke(() =>
                {
                    CacheText.Text = $"{FormatSize(currentSize)} / {CalculateThePercentage(totalSize, currentSize):0.00} %";
                    CacheProgressBar.Value = CalculateThePercentage(totalSize, currentSize);
                });
            }
        });
    }
    void TotalChangeCallback(long totalSize)
    {
        Console.WriteLine($"总大小变化: {FormatSize(totalSize)}");

        Dispatcher.UIThread.Invoke(() => TotalText.Text = FormatSize(totalSize)); 
    }

    static string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;
        
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }
        
        return $"{number:n2} {suffixes[counter]}";
    }

    static double CalculateThePercentage(long totalSize, long currentSize)
    {
        return (double)currentSize / (double)totalSize * 100;
    }
}