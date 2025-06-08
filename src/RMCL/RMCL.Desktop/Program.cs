using System.Diagnostics;
using Avalonia;
using RMCL;
using RMCL.Config;
using RMCL.JavaManager;
using RMCL.Logger;
using RMCL.PathsDictionary;

sealed class Program
{
    public static void Main(string[] args)
    {
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
            Config.LoadConfig();
            JavaManager.LoadConfig();
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