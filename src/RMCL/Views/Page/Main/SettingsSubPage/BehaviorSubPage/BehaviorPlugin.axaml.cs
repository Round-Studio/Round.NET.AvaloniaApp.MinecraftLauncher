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
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Helper;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorPlugin : UserControl
{
    public BehaviorPlugin()
    {
        InitializeComponent();
        var lst = System.IO.Directory.GetFiles(PathsList.PluginPath, "*.rplck").ToList();
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

    private async Task UpdateList()
    {
        var lst = System.IO.Directory.GetFiles(PathsList.PluginPath, "*.rplck").ToList();
        lst.ForEach(file =>
        {
            var info = PluginFileInfoHelper.GetFileInfo(file);

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
                GlobalModels.MainWindow.OpenDraw("",$"插件详细信息：{info.PackName}");
            };
            
            PluginList.Children.Add(item);
        });
    }
}