using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Style.Core;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.View;
using RMCL.Base.Enum.Style;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

public partial class StyleColor : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public StyleColor()
    {
        InitializeComponent();
        ChooseTheme.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.LightThemeType;
        if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Image)
        {
            if (GlobalModels.Config.Data.StyleConfig.BackgroundImages.Count > 0)
            {
                ImageType.IsEnabled = true;
            }
        }
        ChooseColorType.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.AccentColorType;

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

        AccentColor.Colors.ForEach(c => ColorsView.Items.Add(new ItemViewItem()
        {
            Content = new Border()
            {
                Background = Brush.Parse(c),
                CornerRadius = new CornerRadius(8),
            },
            Width = 48,
            Height = 48,
            ClipToBounds = true,
        }));

        ColorsView.SelectedIndex = GlobalModels.Config.Data.StyleConfig.AccentColorIndex;

        IsEditMode = true;
    }

    private void ChooseTheme_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.LightThemeType = (ThemeModelEnum)ChooseTheme.SelectedIndex;
            GlobalModels.Config.Save();
            
            ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.StyleConfig.LightThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        }
    }


    private void ChooseColorType_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.AccentColorType = (AccentColorType)ChooseColorType.SelectedIndex;
            GlobalModels.Config.Save();

            App.LoadColor();
        }
    }

    private void ColorsView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.StyleConfig.AccentColorIndex = ColorsView.SelectedIndex;
            GlobalModels.Config.Save();
            
            App.LoadColor();
        }
    }
}