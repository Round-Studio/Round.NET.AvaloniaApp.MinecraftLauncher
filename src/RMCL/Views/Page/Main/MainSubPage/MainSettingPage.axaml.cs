using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation;
using RMCL.Views.Page.Main.SettingsSubPage;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainSettingPage : UserControl
{
    public static MainSettingPage Page { get; set; }
    public MainSettingPage()
    {
        InitializeComponent();
        Page = this;
        
        SettingsNavigation.NavigateTo(new SettingsNavigation());
    }

    public async Task NavigationTo(object obj)
    {
        SettingsNavigation.NavigateTo(obj);
        if (obj is SettingsNavigation)
        {
            // BackBtn.IsEnabled = false;
            BackBtn.Margin = new Thickness(-80, 23,80,23);
            await Task.Delay(100);
            LineTextBlock.Margin = new Thickness(20);
        }
        else
        {
            // BackBtn.IsEnabled = true;
            LineTextBlock.Margin = new Thickness(64,20,0,20);
            await Task.Delay(100);
            BackBtn.Margin = new Thickness(20, 23);
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigationTo(new SettingsNavigation());
    }
}