using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using OnePointUI.Avalonia.Style.Core;
using RMCL.Base.Enum.Style;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

public partial class StyleColor : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public StyleColor()
    {
        InitializeComponent();
        ChooseTheme.SelectedIndex = (int)GlobalModels.Config.Data.ThemeType;

        IsEditMode = true;
    }

    private void ChooseTheme_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.ThemeType = (ThemeModelEnum)ChooseTheme.SelectedIndex;
            GlobalModels.Config.Save();
            
            ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.ThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        }
    }
}