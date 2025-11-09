using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupCompleted : UserControl
{
    public SetupCompleted()
    {
        InitializeComponent();
    }

    private void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        GlobalModels.Config.Data.FirstRun = false;
        GlobalModels.Config.Save();
        MainView.Instance.GoToMainPage();
    }
}