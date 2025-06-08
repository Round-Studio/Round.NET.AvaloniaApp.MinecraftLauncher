using System.Text.Json;
using System.Text.RegularExpressions;
using RMCL.Base.Entry.Java;
using RMCL.PathsDictionary;

namespace RMCL.JavaManager;

public class JavaManager
{
    private const string           JsonConfigFileName = PathDictionary.JavaConfigPath;
    public static List<JavaDetils> Javas              = new();
    public static JavaRootEntry    JavaRoot           = new();

    public static List<string> KeyWords = new()
    {
        "jvm",
        "roaming",
        "bin",
        "craft",
        "oracle",
        "eclipse",
        "microsoft",
        "jre",
        "temp",
        "program",
        "mc",
        "game",
        "software",
        "hotspot",
        "jdk",
        "run",
        "users",
        "corretto",
        "env",
        "android",
        "file",
        "java",
        "mojang",
        "bin",
        "新建文件夹",
        "jbr",
        "cache",
        "idea"
    };

    public static void LoadConfig()
    {
        if (!File.Exists(Path.GetFullPath(JsonConfigFileName)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JsonConfigFileName));
            var searchJavaAsync = SearchJavaAsync().Result;
            Javas.AddRange(searchJavaAsync);
            JavaRoot.Javas = Javas;
            SaveConfig();
            return;
        }

        var json = File.ReadAllText(Path.GetFullPath(JsonConfigFileName));
        if (string.IsNullOrEmpty(json))
        {
            var searchJavaAsync = SearchJavaAsync().Result;
            Javas.AddRange(searchJavaAsync);
            JavaRoot.Javas = Javas;
            SaveConfig();
        }
        else
        {
            try
            {
                var detilsArray = JsonSerializer.Deserialize<JavaRootEntry>(json);
                if (detilsArray.Javas.Count == 0)
                {
                    var searchJavaAsync = SearchJavaAsync().Result;
                    Javas.AddRange(searchJavaAsync);
                    JavaRoot.Javas = Javas;
                    SaveConfig();
                }

                Javas.AddRange(detilsArray.Javas);
            }
            catch
            {
                SaveConfig();
            }
        }
    }

    public static Task<JavaDetils[]> SearchJavaAsync()
    {
        List<string> path = new();
        List<JavaDetils> javas = new();
        if (OperatingSystem.IsWindows())
        {
            //Env 定不为null
            var env = Environment.GetEnvironmentVariable("Path");
            var paths = env.Split(Path.PathSeparator);
            foreach (var i in paths)
            {
                var java_path = Path.Combine(i, "java.exe");
                //虽然但是，我觉得应该没问题
                if (File.Exists(java_path)) path.Add(java_path);
            }

            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
                if (drive.IsReady)
                {
                    var driveRootDirectory = drive.RootDirectory;
                    try
                    {
                        var windowsSearchJava = WindowsSearchJava(drive.RootDirectory);
                        path.Any(c =>
                                 {
                                     if (windowsSearchJava.Contains(c)) return windowsSearchJava.Remove(c);
                                     return true;
                                 });
                        path.AddRange(windowsSearchJava);
                    }
                    catch
                    {
                    }
                }

            var handleJavaPaths = HandleJavaPaths(path);
            foreach (var javaPath in handleJavaPaths)
            {
                var detils = new JavaDetils(javaPath);
                javas.Add(detils);
            }
        }

        if (OperatingSystem.IsMacOS())
        {
        }

        if (OperatingSystem.IsLinux())
        {
        }

        return Task.FromResult(javas.ToArray());
    }

    /// <summary>
    ///     Windows搜索
    /// </summary>
    /// <returns></returns>
    private static List<string> WindowsSearchJava(DirectoryInfo pathInfo)
    {
        List<string> paths = new();
        try
        {
            var path = Path.Combine(pathInfo.FullName, "java.exe");
            if (File.Exists(path)) paths.Add(path);
            //继续递归，避免有笨蛋套娃Java
            foreach (var directoryInfo in pathInfo.EnumerateDirectories())
                //判断是否为符号链接
                if (!directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    var name = directoryInfo.Name.ToLower(); //小写化字母
                    if (KeyWords.Any(name.Contains))
                    {
                        var list = WindowsSearchJava(directoryInfo);
                        paths.AddRange(list);
                    }
                }
        }
        catch
        {
        }

        return paths;
    }

    private static JavaInformation[] HandleJavaPaths(List<string> pathsList)
    {
        List<JavaInformation> javalists = [];
        foreach (var java in pathsList)
        {
            var parent = Directory.GetParent(java);
            var javaw = Path.Combine(parent.FullName, OperatingSystem.IsWindows() ? "javaw.exe" : "javaw");
            var javac = Path.Combine(parent.FullName, OperatingSystem.IsWindows() ? "javac.exe" : "javac");

            var readRelease = ReadRelease(parent.FullName + Path.DirectorySeparatorChar);
            if (OperatingSystem.IsWindows() && readRelease.VERSION == null)
            {
                var dataTuple = Utils.RunWindows(java, "-version");
                var version = Regex.Match(dataTuple.errorPut.ToLower(), """version\s+"([\d._]+)""").Groups[1].Value;
                var javaInformation = new JavaInformation
                {
                    Implementor = "Unknown",
                    Java = java,
                    JavaW = javaw,
                    Version = version
                };
                javalists.Add(javaInformation);
            }
            else
            {
                var information = new JavaInformation
                {
                    Implementor = readRelease.IMPLEMENTOR,
                    Java = java,
                    JavaW = javaw,
                    Version = readRelease.VERSION
                };
                javalists.Add(information);
            }
        }

        return javalists.ToArray();
    }

    private static (string IMPLEMENTOR, string VERSION, string ARCH, string SYSTEM) ReadRelease(string java_parent)
    {
        if (java_parent.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
            java_parent = java_parent.Replace(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                string.Empty);
        var combine = Path.Combine(java_parent, "release");
        if (!File.Exists(combine)) return (null, null, null, null);

        string implementor, version, arch, system;
        implementor = version = arch = system = string.Empty;
        var fileLines = File.ReadAllLines(combine);
        foreach (var line in fileLines)
        {
            if (line.StartsWith("IMPLEMENTOR")) implementor = line.Split("=")[1].Trim('"');

            if (line.StartsWith("JAVA_VERSION=") || line.StartsWith("JAVA_VERSION ="))
                version = line.Split("=")[1].Trim('"');

            if (line.StartsWith("OS_ARCH")) arch = line.Split("=")[1].Trim('"');

            if (line.StartsWith("OS_NAME")) system = line.Split("=")[1].Trim('"').ToLower();
        }

        return (implementor, version, arch, system);
    }

    public static void SaveConfig()
    {
        var jsresult =
            JsonSerializer.Serialize(JavaRoot, new JsonSerializerOptions { WriteIndented = true }); //获取结果并转换成正确的格式
        File.WriteAllText(Path.GetFullPath(JsonConfigFileName), jsresult);
    }
}