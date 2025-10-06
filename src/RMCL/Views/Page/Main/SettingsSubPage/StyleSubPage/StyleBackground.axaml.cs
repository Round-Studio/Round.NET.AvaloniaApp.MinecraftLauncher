using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

public partial class StyleBackground : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public StyleBackground()
    {
        InitializeComponent();
        ChooseBackMaterial.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.BackMaterialType;
        BackgroundTypeBox.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.StyleType;

        if (OperatingSystem.IsWindows())
        {
            var osVersion = Environment.OSVersion;
            int buildNumber = osVersion.Version.Build;

            // Windows 版本判断逻辑
            if (osVersion.Version.Major == 10)
            {
                if (buildNumber >= 22000) // Win11
                {
                    MicaModel.IsEnabled = true;
                    BlurModel.IsEnabled = true;
                }
                else if (buildNumber >= 10240) // Win10
                {
                    BlurModel.IsEnabled = true;
                }
            }
        }
        
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Style,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new SettingsStyle());
                }
            },
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_Style_Background,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new StyleBackground());
                }
            }
        });

        IsEditMode = true;
    }

    private void ChooseBackMaterial_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.BackMaterialType = (BackMaterialHelper.BackMaterialType)ChooseBackMaterial.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();
        }
    }

    private void BackgroundTypeBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.StyleType = (StyleType)BackgroundTypeBox.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();
        }
    }
}