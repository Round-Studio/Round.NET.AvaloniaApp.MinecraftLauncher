using Avalonia;
using System;
using System.Threading;
using HarfBuzzSharp;
using RMCL.Base.Entry.Config;
using RMCL.Config;
using RMCL.Models;
using RMCL.Models.Global;

namespace RMCL;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        GlobalModels.Config = new Config<ConfigEntry>(PathsList.ConfigPath);

        if (args.Length <= 0)
        {
            // Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-hans"); // 简体中文
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageHelper.GetStringName(GlobalModels.Config.Data.Language));
        
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}