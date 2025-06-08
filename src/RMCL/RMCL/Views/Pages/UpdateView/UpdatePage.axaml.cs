using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Base.Entry.Update;
using RMCL.Controls.Download;
using RMCL.Controls.TaskContentControl;
using RMCL.Models.Classes;
using RMCL.Models.Classes.Manager.TaskManager;
using Path = System.IO.Path;

namespace RMCL.Views.Pages.UpdateView;

public partial class UpdatePage : UserControl
{
    private UpdateEntry.GitHubRelease _updateRelease;
    private string _updateUrl;
    public UpdatePage(string url,UpdateEntry.GitHubRelease entry)
    {
        _updateRelease = entry;
        _updateUrl = url;
        InitializeComponent();
        
        Publisher.Text = $"发布者：{entry.Author.Login}";
        Released.Text = entry.PublishedAt.ToString();

        CurrentVersion.Text = $"当前版本：{Assembly.GetEntryAssembly()?.GetName().Version.ToString()}";
        UpdatedVersion.Text = $"更新版本：{entry.Name.Replace("v", "")}";
        VersionBranch.Text =  $"更新分支：{entry.TagName.Replace("RMCL.", "").Split('-')[0]}";
    }

    private void OpenALinkToAWebPage_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemHelper.OpenUrl(_updateRelease.HtmlUrl);
    }

    private void OnUpdateBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var dow = new DownloadUpdateFileTaskItem();
        var cont = new TaskControl()
        {
            BoxContent = dow,
            TaskName = $"下载更新 - {_updateRelease.Name}"
        };
        cont.RunTask();
        dow.FileUrl = _updateUrl;
        dow.FileName = Path.Combine(PathsDictionary.PathDictionary.UpdateZipFileFolder,$"{_updateRelease.TagName}.zip");
        dow.InstallName = Path.Combine(PathsDictionary.PathDictionary.UpdateFileFolder,$"{_updateRelease.TagName}");
        dow.StartDownload();
        var uuid1 = TaskManager.AddTask(cont);
        dow.DownloadCompleted = () => TaskManager.DeleteTask(uuid1);
        
        Core.ChildFrame.Close();
    }
}