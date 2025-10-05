using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Style.Core;
using RMCL.Base.Enum.Style;
using RMCL.Models.Global;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

public partial class StyleColor : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public StyleColor()
    {
        InitializeComponent();
        ChooseTheme.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.ThemeType;
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
                ItemName = Resource.Settings_Style_Color,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new StyleColor());
                }
            }
        });

        IsEditMode = true;
    }

    private void ChooseTheme_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.ThemeType = (ThemeModelEnum)ChooseTheme.SelectedIndex;
            GlobalModels.Config.Save();
            
            ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.StyleConfig.ThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        }
    }
}