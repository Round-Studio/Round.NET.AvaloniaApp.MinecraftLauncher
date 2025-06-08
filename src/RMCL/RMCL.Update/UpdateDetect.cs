using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Newtonsoft.Json;
using RMCL.Base.Entry.Update;
using RMCL.Base.Enum.Update;

namespace RMCL.Update;

public class UpdateDetect
{
    public string ApiUrl { get; set; } = string.Empty;
    private HttpClient _client;
    private List<string> Branch = new List<string>() { "Release", "Transcend" };
    public UpdateBranch BranchIndex { get; set; } = UpdateBranch.Release;
    public UpdateEntry.GitHubRelease Entry { get; set; }
    public string NowVersion { get; } = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
    public Action<string,UpdateEntry.GitHubRelease> OnUpdate { get; set; } = (url,entry) => { };

    public UpdateDetect(string apiUrl)
    {
        ApiUrl = apiUrl;
        
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", $"RMCL-Update/{NowVersion}"); // GitHub API 要求 User-Agent 头
    }

    public async Task Detect()
    {
        var copynowver = NowVersion;

        if (NowVersion.Split('.')[1].Length == 3)
        {
            var lst = NowVersion.Split('.');
            lst[1] = $"0{lst[1]}";
            copynowver = string.Join(".", lst);

            Entry.Name = $"v{copynowver}";
        }
        
        var json = await _client.GetAsync(ApiUrl).Result.Content.ReadAsStringAsync();
        
        Console.WriteLine($"Update Json: {json}");

        Entry = JsonConvert.DeserializeObject<UpdateEntry.GitHubRelease>(json);
        Console.WriteLine($"Update Tag: {Entry.TagName}");
        Console.WriteLine($"Now Version: {copynowver}");

        var index = BranchIndex.GetHashCode();
        var tagName = Branch[index];

        if (Entry.TagName.Contains(tagName))
        {
            if (!Entry.TagName.Contains(copynowver))
            {
                var res = GetSystemApplication();
                Console.WriteLine($"Update File URL: {res}");
                OnUpdate(res, Entry);
            }
        }
    }

    private string GetSystemApplication()
    {
        var arch = GetSystemInformation();
        string resurl = "";
        Entry.Assets.ForEach(x =>
        {
            if (OperatingSystem.IsWindows() && x.Name.Contains("win") && x.Name.Contains(arch))
            {
                resurl = x.BrowserDownloadUrl;
            }
            
            if (OperatingSystem.IsLinux() && x.Name.Contains("linux") && x.Name.Contains(arch))
            {
                resurl = x.BrowserDownloadUrl;
            }
            
            if (OperatingSystem.IsMacOS() && x.Name.Contains("osx") && x.Name.Contains(arch))
            {
                resurl = x.BrowserDownloadUrl;
            }
        });
        
        return resurl;
    }

    private string GetSystemInformation()
    {
        string osArch = RuntimeInformation.OSArchitecture.ToString();
        Console.WriteLine($"操作系统架构: {osArch}");
        
        return osArch.ToLower();
    }
}