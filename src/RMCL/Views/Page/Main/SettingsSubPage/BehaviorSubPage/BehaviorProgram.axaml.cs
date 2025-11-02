using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.BehaviorSubPage;

public partial class BehaviorProgram : UserControl
{
    public bool IsEdit = false;
    public BehaviorProgram()
    {
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            CardOnline.IsVisible = true;
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
                ItemName = "程序与功能",
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new BehaviorProgram());
                }
            }
        });

        ToggleOnline.IsChecked = GlobalModels.Config.Data.ProgramConfig.ToggleOnline;
        TogglePlugin.IsChecked = GlobalModels.Config.Data.ProgramConfig.TogglePlugin;

        IsEdit = true;
    }

    private void ToggleOnline_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.ProgramConfig.ToggleOnline = (bool)ToggleOnline.IsChecked;
            GlobalModels.Config.Save();
            
            MainSettingPage.Page.SetReStart();
        }
    }

    private void TogglePlugin_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.ProgramConfig.TogglePlugin = (bool)TogglePlugin.IsChecked;
            GlobalModels.Config.Save();
            
            MainSettingPage.Page.SetReStart();
        }
    }
}