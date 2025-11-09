using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Interface;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupProgram : ISetting
{
    public SetupProgram()
    {
        InitializeComponent();
        TogglePlugin.IsChecked = GlobalModels.Config.Data.ProgramConfig.TogglePlugin;
        ToggleOnline.IsChecked = GlobalModels.Config.Data.ProgramConfig.ToggleOnline;
        
        IsEdit = true;
    }

    private void TogglePlugin_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.ProgramConfig.TogglePlugin = (bool)TogglePlugin.IsChecked;
            GlobalModels.Config.Save();
        }
    }

    private void ToggleOnline_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.ProgramConfig.ToggleOnline = (bool)ToggleOnline.IsChecked;
            GlobalModels.Config.Save();
        }
    }
}