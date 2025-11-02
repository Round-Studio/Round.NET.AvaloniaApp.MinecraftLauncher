using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Models.Global;
using RMCL.Models.Plugin;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorPlugin : UserControl
{
    public BehaviorPlugin()
    {
        InitializeComponent();
        if (LoadPlugins.Plugins.Count == 0)
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
    }

    private async Task UpdateList()
    {
        
    }
}