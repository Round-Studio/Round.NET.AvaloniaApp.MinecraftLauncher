using Avalonia;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HarfBuzzSharp;
using OverrideLauncher.Core.Base.Dictionary;
using RMCL.Base.Entry.Config;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using Round.SDK.Entity;
using Round.SDK.Logger;

namespace RMCL;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        GlobalModels.Config = new ConfigEntity<ConfigEntry>(PathsList.ConfigPath);
        
        var version = Assembly.GetEntryAssembly().GetName().Version;

        if (args.Length <= 0)
        {
            ConsoleRedirector consoleRedirector = new ConsoleRedirector(Path.Combine(PathsList.LogPath, "Client",
                $"[RMCL.Logger] {DateTime.Now.ToString("yyyy.MM.dd HHmmss.fff")}.log"));
            Console.WriteLine(@"RMCL 客户端启动");
            Console.WriteLine($@"当前程序版本：{version}");
            ConsoleRedirector.RegisterThread(Thread.CurrentThread,"Program");
            
            Console.WriteLine(@"Main 入口启动");

            if (false)
            {
                Task.Run(() =>
                {
                    ConsoleRedirector.RegisterThread(Thread.CurrentThread,"Server");
                    Console.WriteLine(@"启动后台服务器...");
                    // 获取当前应用程序的路径和文件名
                    string applicationPath = Process.GetCurrentProcess().MainModule.FileName;

                    // 启动新的应用程序实例
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = applicationPath,
                        UseShellExecute = true,
                        ArgumentList = { "-server" }
                    };
                    Console.WriteLine(@"初始化后台程序");

                    // 启动新实例
                    Process.Start(startInfo);
                    Console.WriteLine(@"服务器启动完毕。");
                });
            }
            
            DictionaryDownloadHost.SwitchMirror("official");
            Console.WriteLine(@"Config 读取完毕，即将启动 Avalonia 桌面程序。");

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        else
        {
            ConsoleRedirector consoleRedirector = new ConsoleRedirector(Path.Combine(PathsList.LogPath, "Server",
                $"[RMCL.Logger] {DateTime.Now.ToString("yyyy.MM.dd HHmmss.fff")}.log"));
            Console.WriteLine(@"RMCL 服务端启动");
            Console.WriteLine($@"当前程序版本：{version}");
            
            while(true) { }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}