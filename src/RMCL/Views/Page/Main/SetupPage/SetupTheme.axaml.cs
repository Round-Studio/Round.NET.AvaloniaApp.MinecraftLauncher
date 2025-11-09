using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using OnePointUI.Avalonia.Style.Core;
using RMCL.Base.Enum.Style;
using RMCL.Interface;
using RMCL.Models;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupTheme : ISetting
{
    public SetupTheme()
    {
        InitializeComponent();
        ChooseMaterialBox.SelectedIndex = (int)GlobalModels.Config.Data.StyleConfig.BackMaterialType;
        ChooseThemeBox.SelectedIndex =  (int)GlobalModels.Config.Data.StyleConfig.LightThemeType;
        
        IsEdit = true;
    }

    private void ChooseThemeBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.StyleConfig.LightThemeType = (ThemeModelEnum)ChooseThemeBox.SelectedIndex;
            GlobalModels.Config.Save();
            
            ThemeManager.Instance.SetThemeModel(GlobalModels.Config.Data.StyleConfig.LightThemeType == ThemeModelEnum.Light ? ThemeVariant.Light : ThemeVariant.Dark);
        }
    }

    private void ChooseMaterialBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.StyleConfig.BackMaterialType = (BackMaterialHelper.BackMaterialType)ChooseMaterialBox.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();
        }
    }
}