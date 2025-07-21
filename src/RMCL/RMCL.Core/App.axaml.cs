using System;
using System.Drawing;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using HotAvalonia;
using NetworkService.SingleInstanceDetector;
using RMCL.Base.Entry.Style;
using RMCL.Config;
using RMCL.Core.Views;
using RMCL.Core.Views.Windows;
using RMCL.Core.Views.Windows.Initialize;
using Tmds.DBus.Protocol;
using Color = Avalonia.Media.Color;

namespace RMCL.Core;

public partial class App : Application
{
    public override void Initialize()
    {
        Console.WriteLine("App Init...");
        
        Task.Run(() =>
        {
            while (true)
            {
                if (Config.Config.MainConfig.GCTime != 999999999999999999)
                {
                    Thread.Sleep((int)Config.Config.MainConfig.GCTime);
                    GC.Collect(2, GCCollectionMode.Aggressive, true);
                }
            }
        });
        
        // 订阅所有全局异常处理器
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        Dispatcher.UIThread.UnhandledException += UIThread_UnhandledException;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("On Framework Initialization Completed");
        DisableAvaloniaDataAnnotationValidation();
        this.Styles.Add(Core.Models.Classes.Core.FluentAvaloniaTheme);

        if (!Config.Config.MainConfig.FirstLauncher)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
        }
        else
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new InitializeWindow();
            }
        }

        RequestedThemeVariant = Config.Config.MainConfig.ThemeColors.Theme switch
        {
            ThemeType.Dark => ThemeVariant.Dark,
            ThemeType.Light => ThemeVariant.Light
        };

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        LogException(exception, "AppDomain");
        ShowErrorDialog(exception);
        
        // 非致命错误可以继续运行
        if (!e.IsTerminating)
        {
            return;
        }
        
        // 致命错误需要特殊处理
        // HandleFatalError(exception);
        return;
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        LogException(e.Exception, "Task");
        ShowErrorDialog(e.Exception);
        e.SetObserved(); // 标记为已处理
    }

    private void ShowErrorDialog(Exception ex)
    {
        try
        {
            var time = DateTime.Now;
            Dispatcher.UIThread.Invoke(() =>
            {
                var error = new ErrorWindow();
                error.ShowEx(ex,time);
                
                //if (Config.MainConfig.ShowErrorWindow)
                //{
                error.Show();
                // }
            });
        }
        catch (Exception dialogEx)
        {
            // 如果对话框本身出错，至少记录到控制台
            Console.WriteLine($"显示错误对话框时出错: {dialogEx}");
        }
    }

    private void LogException(Exception ex, string source)
    {
        Console.WriteLine($"{source} 异常: {ex?.ToString() ?? "未知异常"}");
        // 这里可以添加文件日志记录
    }

    private void UIThread_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogException(e.Exception, "UI Thread");
        ShowErrorDialog(e.Exception);
        e.Handled = true; // 阻止应用崩溃
    }

    private void HandleFatalError(Exception ex)
    {
        LogException(ex, "FATAL");
        // 尝试保存重要数据
        // ...
        
        // 优雅退出
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown(1);
        }
        else
        {
            Environment.Exit(1);
        }
    }
}