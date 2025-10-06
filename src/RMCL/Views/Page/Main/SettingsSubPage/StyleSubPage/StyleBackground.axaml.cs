using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Control.Item;
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
        UpdateUI();

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

    private void UpdateUI()
    {
        IsEditMode = false;
        BackgroundImageBox.IsVisible = false;
        
        if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Image)
        {
            BackgroundImageBox.IsVisible = true;
            BackgroundsList.SelectedIndex = -1;
            BackgroundsList.Items.Clear();
            
            GlobalModels.Config.Data.StyleConfig.BackgroundImages.ForEach(image =>
            {
                var item = new BackgroundChooseItem() { ImagePath = image };
                item.UpdateUI();
                BackgroundsList.Items.Add(item);
            });

            var index = GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex;
            if (index != -1)
            {
                if (GlobalModels.Config.Data.StyleConfig.BackgroundImages.Count >= 0)
                {
                    BackgroundsList.SelectedIndex = index;
                }
            }
        }
        
        IsEditMode = true;
    }
    private void BackgroundTypeBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.StyleType = (StyleType)BackgroundTypeBox.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();

            UpdateUI();
        }
    }

    private void BackgroundsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex = BackgroundsList.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();
            UpdateUI();
        }
    }

    private async void ImportBackgroundBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
    
        // 2. 配置文件选择器选项
        var filePickerOptions = new FilePickerOpenOptions
        {
            Title = "Choose Image File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                FilePickerFileTypes.ImageAll
            }
        };
    
        // 3. 打开对话框并获取文件
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(filePickerOptions);
    
        // 4. 处理选中的文件
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                // 获取文件路径
                string filePath = file.Path.LocalPath;
                
                GlobalModels.Config.Data.StyleConfig.BackgroundImages.Add(filePath);
                UpdateUI();
            }
        }
        else
        {
            // 用户取消了选择
            Console.WriteLine("未选择文件。");
        }
    }
}