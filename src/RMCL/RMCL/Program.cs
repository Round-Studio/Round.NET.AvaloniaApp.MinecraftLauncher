using System.Diagnostics;
using System.Runtime;
using Avalonia;
using RMCL.Base.Entry.Game.GameDrawer;
using RMCL.Core;
using RMCL.Config;
using RMCL.Core.Models.Classes.Manager.GameDrawerManager;
using RMCL.JavaManager;
using RMCL.Logger;
using RMCL.Core.Models.Classes.Manager.UserManager;
using RMCL.PathsDictionary;

sealed class Program
{
    public static void Main(string[] args)
    {
        // 优化GC设置以提高性能
        System.Runtime.GCSettings.LatencyMode = GCLatencyMode.Interactive;

        // 设置进程优先级
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

        // 预热线程池
        ThreadPool.SetMinThreads(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        
        if (args.Length >= 4)
        {
            string bt = "";
            string neobt = "";

            for (int i = 0; i < args.Length; i++)
            {
                var se = args[i];
                if (se == "-install") neobt = args[i + 1];
                else if (se == "-body") bt = args[i + 1];
            }

            try
            {
                Thread.Sleep(200); // 确保当前程序已经释放对文件的占用

                // 检查文件是否存在
                if (File.Exists(bt))
                {
                    // 尝试删除旧文件
                    File.Delete(bt);
                }

                // 复制新文件
                File.Copy(neobt, bt, true);

                // 启动新文件
                Process.Start(bt);

                // 退出当前程序
                Environment.Exit(0);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"权限不足: {ex.Message}");
                Console.WriteLine("请以管理员身份运行此程序。");
                Environment.Exit(1); // 退出程序，提示用户以管理员身份运行
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新失败: {ex.Message}");
                Environment.Exit(1); // 退出程序
            }
        }
        else
        {
            var redirector = new ConsoleRedirector(Path.GetFullPath(Path.Combine(PathDictionary.LogsPath,$"[RMCL.Logger] {DateTime.Now.ToString("yyyy.MM.dd HHmmss.fff")}.log")));
        
            Console.WriteLine("Program Starting!");
            Console.WriteLine("Program Load Config...");
            Config.LoadConfig();
            Console.WriteLine("Program Load JavaConfig...");
            JavaManager.LoadConfig();
            Console.WriteLine("Program Load PlayerConfig...");
            PlayerManager.LoadConfig();
            Console.WriteLine("Program Load GameDrawerConfig...");
            GameDrawerManager.LoadConfig();
            
            Console.WriteLine("Program Init...");
        
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