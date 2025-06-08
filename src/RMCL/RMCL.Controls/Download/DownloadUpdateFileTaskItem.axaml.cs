using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Downloader;

namespace RMCL.Controls.Download;

public partial class DownloadUpdateFileTaskItem : UserControl
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string InstallName { get; set; } = string.Empty;
    public Action DownloadCompleted;
    public DownloadUpdateFileTaskItem()
    {
        InitializeComponent();
    }

    public void StartDownload()
    {
        DownloadService down = new DownloadService();
        down.DownloadFileCompleted += (s, e) => InstallUpdate();
        down.DownloadProgressChanged += (s, e) =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                DownloadProgress.Value = e.ProgressPercentage;
                ProgressTextBox.Text = $"下载更新 {e.ProgressPercentage:0.00}%";
            });
        };
        down.DownloadFileTaskAsync(FileUrl, FileName);
    }

    public void InstallUpdate()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            DownloadProgress.Value = 40;
            ProgressTextBox.Text = $"解压更新文件...";
        });
        if (!File.Exists(FileName))
        {
            DownloadCompleted();
        }

        Directory.CreateDirectory(Path.GetFullPath(InstallName));
        ZipFile.ExtractToDirectory(FileName, Path.GetFullPath(InstallName),true);
        
        var fils = Directory.GetFiles(Path.GetFullPath(InstallName));
        var file = "";
        foreach (var f in fils) if(Path.GetFileName(f).StartsWith("RMCL.Desktop")) file = f;

        Console.WriteLine(file);
        Process.Start(file, ["-install", $"{file}", "-body", Process.GetCurrentProcess().MainModule.FileName]);
        
        Thread.Sleep(50);
        
        Environment.Exit(0);
    }
}