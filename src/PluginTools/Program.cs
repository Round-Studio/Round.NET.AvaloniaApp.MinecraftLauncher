using System.Diagnostics;
using PluginTools.Entry;
using Round.SDK.Entity;
using Round.SDK.Logger;

namespace PluginTools;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("请提供参数。\n" +
                              "可通过 -h 或 -help 命令查看参数列表及用法");
            
            return;
        }

        if (args.Contains("-h") || args.Contains("--help"))
        {
            Console.WriteLine("PluginTools 帮助列表\n\n" +
                              "-h / -help => PluginTools 帮助列表\n" +
                              "-c / -creat => 创建一个插件包配置文件模版\n" +
                              "-b / -build -config <配置文件> => 根据配置文件生成插件包");
        }

        if (args.Contains("-c") || args.Contains("--creat"))
        {
            Console.WriteLine("PluginTools 创建新插件包配置文件\n");

            Console.Write("插件包名称：");
            var projectName = Console.ReadLine();
            Console.Write("配置文件输出地址：");
            var projectFilePath = Console.ReadLine();

            var Config = new ConfigEntity<ConfigFileEntry>(Path.Combine(projectFilePath, projectName + ".json"));
            Config.Load();
            Config.Data.PackName = projectName;
            Config.Save();
            Console.WriteLine($"配置文件已生成到：{Config.Path}");
        }

        if (args.Contains("-b") || args.Contains("--build"))
        {
            var configFile = args[args.ToList().FindIndex(x => x.StartsWith("-config")) + 1];
            var Config = new ConfigEntity<ConfigFileEntry>(configFile);
            Config.Load();

            if (Directory.Exists(Config.Data.BuildOutputPath)) Directory.Delete(Config.Data.BuildOutputPath, true);

            var buildCommand =
                $"publish \"{Config.Data.BuildProjectFilePath}\" -c Release -o \"{Path.Combine(Config.Data.BuildOutputPath, "files")}\"";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = buildCommand
                }
            };
            process.Start();
            process.WaitForExit();

            Directory.CreateDirectory(Path.Combine(Config.Data.BuildOutputPath, "assets"));
            Directory.CreateDirectory(Path.Combine(Config.Data.BuildOutputPath, "assets", "screenshots"));
        }
    }
}