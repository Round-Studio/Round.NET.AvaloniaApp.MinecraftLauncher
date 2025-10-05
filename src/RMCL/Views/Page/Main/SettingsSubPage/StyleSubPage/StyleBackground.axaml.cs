using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
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
}