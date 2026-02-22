using Avalonia.Controls;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Notice.Info;
using RMCL.Base.Entry.Config;
using RMCL.Views.Windows;
using Round.SDK.Entity;

namespace RMCL.Models.Global;

public class GlobalModels
{
    public static ConfigEntity<ConfigEntry>? Config { get; set; }
    public static MainWindow? MainWindow;
}