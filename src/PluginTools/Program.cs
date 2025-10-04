using System.Diagnostics;
using System.Net;
using PluginTools.Entry;
using Round.SDK.Entity;
using Round.SDK.Helper;
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
            Console.WriteLine(@"PluginTools 创建新插件包配置文件
");

            Console.Write("插件包名称：");
            var projectName = Console.ReadLine();
            Console.Write("配置文件输出地址：");
            var projectFilePath = Console.ReadLine();

            var Config = new ConfigEntity<ConfigFileEntry>(Path.Combine(projectFilePath, projectName + ".json"));
            Config.Load();
            Config.Data.PackName = projectName;
            Config.Save();
            Console.WriteLine($@"配置文件已生成到：{Config.Path}");
        }

        if (args.Contains("-b") || args.Contains("--build"))
        {
            var configFile = args[args.ToList().FindIndex(x => x.StartsWith("-config")) + 1];
            var Config = new ConfigEntity<ConfigFileEntry>(configFile);
            Config.Load();

            if (Directory.Exists(Config.Data.BuildOutputPath)) Directory.Delete(Config.Data.BuildOutputPath, true);

            var buildCommand =
                $"publish \"{Config.Data.BuildProjectFilePath}\" -c Release -o \"{Path.Combine(Config.Data.BuildOutputPath, "build", "files")}\"";

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

            Directory.CreateDirectory(Path.Combine(Config.Data.BuildOutputPath, "build", "assets"));
            Directory.CreateDirectory(Path.Combine(Config.Data.BuildOutputPath, "build", "assets", "screenshots"));
            Directory.CreateDirectory(Path.Combine(Config.Data.BuildOutputPath, "build", "assets", "icon"));

            Config.Data.PackScreenshots.ForEach(x =>
            {
                var fileName = Path.GetFileName(x);
                var filePath = Path.Combine(Config.Data.BuildOutputPath, "build", "assets", "screenshots", fileName);
                File.Copy(x, filePath);
            });

            if (!string.IsNullOrEmpty(Config.Data.PackIconPath))
                File.Copy(Config.Data.PackIconPath,
                    Path.Combine(Config.Data.BuildOutputPath, "build", "assets", "icon",
                        Path.GetFileName(Config.Data.PackIconPath)));

            var packConfig = new PackConfig()
            {
                PackName = Config.Data.PackName,
                PackDescription = Config.Data.PackDescription,
                PackIconPath = Path.GetFileName(Config.Data.PackIconPath),
                PackAuthor = Config.Data.PackAuthor,
                PackLicense = Config.Data.PackLicense,
                PackLicenseUrl = Config.Data.PackLicenseUrl,
                PackVersion = Config.Data.PackVersion,
                BodyFile = Config.Data.BodyFile
            };

            var packConfigBody =
                new ConfigEntity<PackConfig>(Path.Combine(Config.Data.BuildOutputPath, "build", "pack.json"));
            packConfigBody.Data = packConfig;
            packConfigBody.Save();

            ZipHelper.CreateZipFile(Path.Combine(Config.Data.BuildOutputPath, "build"),
                Path.Combine(Config.Data.BuildOutputPath, "pack.rplck"));
            
            Directory.Delete(Path.Combine(Config.Data.BuildOutputPath, "build"), true);
            
            Console.WriteLine($@"包已生成至：{Path.Combine(Config.Data.BuildOutputPath, "pack.rplck")}");
        }
    }
}