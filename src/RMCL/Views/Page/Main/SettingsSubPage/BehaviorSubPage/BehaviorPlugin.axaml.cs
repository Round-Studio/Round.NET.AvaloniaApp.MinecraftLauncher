using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using HarfBuzzSharp;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls;
using RMCL.Models.Global;
using RMCL.Models.Plugin;
using RMCL.Properties;
using RMCL.Views.Page.DrawerContent.Setting.Behavior;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Helper;
using System.IO;
using Avalonia.Threading;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorPlugin : UserControl
{
    private FileSystemWatcher _fileSystemWatcher;

    public BehaviorPlugin()
    {
        InitializeComponent();
        InitializeFileWatcher();
        UpdateVisibility();
        
        if (!GlobalModels.Config.Data.ProgramConfig.TogglePlugin)
        {
            EnableTip.IsVisible = true;
        }
        
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Behavior,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsBehavior());
                }
            },
            new BreadcrumbItemInfo()
            {
                ItemName = "插件与脚本",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new BehaviorPlugin());
                }
            }
        });

        UpdateList();
    }

    private void InitializeFileWatcher()
    {
        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = PathsList.PluginPath,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            Filter = "*.*",
            EnableRaisingEvents = true
        };

        // 监听文件变化事件
        _fileSystemWatcher.Changed += OnPluginDirectoryChanged;
        _fileSystemWatcher.Created += OnPluginDirectoryChanged;
        _fileSystemWatcher.Deleted += OnPluginDirectoryChanged;
        _fileSystemWatcher.Renamed += OnPluginDirectoryChanged;
    }

    private void OnPluginDirectoryChanged(object sender, FileSystemEventArgs e)
    {
        // 在UI线程上执行更新
        Dispatcher.UIThread.Post(() =>
        {
            RefreshPluginList();
        });
    }

    private void UpdateVisibility()
    {
        var lst = Directory.GetFiles(PathsList.PluginPath).ToList();
        if (lst.Count == 0)
        {
            NullBox.IsVisible = true;
            PluginViewer.IsVisible = false;
        }
        else
        {
            NullBox.IsVisible = false;
            PluginViewer.IsVisible = true;
        }
    }

    private void RefreshPluginList()
    {
        // 清空当前列表
        PluginList.Children.Clear();
        
        // 更新可见性
        UpdateVisibility();
        
        // 重新加载列表
        UpdateList();
    }

    private async Task UpdateList()
    {
        var lst = Directory.GetFiles(PathsList.PluginPath).ToList();
        lst.ForEach(file =>
        {
            var info = PluginFileInfoHelper.GetFileInfo(file);
            info.PackFile = file;

            var item = new SettingCard()
            {
                Header = info.PackName,
                Description = $"{info.PackDescription}\n{info.PackAuthor} - {info.PackVersion}",
                Margin = new Thickness(5),
                Glyph = "\uEA86",
                IsClickable = true
            };

            if (!string.IsNullOrEmpty(info.PackIconPath))
            {
                item.IsFontIcon = false;
                item.ImageIcon = new Bitmap(info.PackIconPath);
            }

            item.Click += (sender, args) =>
            {
                GlobalModels.MainWindow.OpenDraw(new DrawerBehaviorPluginInformationContent(info),$"插件详细信息：{info.PackName}");
            };

            if (file.EndsWith(".disable"))
            {
                item.Content = new LabelBox()
                {
                    Text = "已禁用",
                    FontSize = 13,
                };
            }
            
            PluginList.Children.Add(item);
        });
    }

    // 释放资源
    public void Dispose()
    {
        _fileSystemWatcher?.Dispose();
    }
}