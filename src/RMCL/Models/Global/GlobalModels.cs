using Avalonia.Controls;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Notice.Info;
using RMCL.Base.Entry.Config;
using RMCL.Views;
using RMCL.Views.Control.Navigation;
using RMCL.Views.Control.Tasks;
using Round.SDK.Entity;

namespace RMCL.Models.Global;

public class GlobalModels
{
    public static ConfigEntity<ConfigEntry>? Config { get; set; }
    public static MainWindow? MainWindow;
    public static NavigationFrame? MainPageContent;
    public static NoticePanel? NoticePanel;
    public static TaskPanel? TaskPanel;
}