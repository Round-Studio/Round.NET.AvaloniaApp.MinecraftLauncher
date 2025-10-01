using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Models;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SettingsSubPage.StyleSubPage;

public partial class StyleBackground : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public StyleBackground()
    {
        InitializeComponent();
        ChooseBackMaterial.SelectedIndex = (int)GlobalModels.Config.Data.BackMaterialType;

        IsEditMode = true;
    }

    private void ChooseBackMaterial_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.BackMaterialType = (BackMaterialHelper.BackMaterialType)ChooseBackMaterial.SelectedIndex;
            GlobalModels.Config.Save();
            
            GlobalModels.MainWindow.UpdateBack();
        }
    }
}