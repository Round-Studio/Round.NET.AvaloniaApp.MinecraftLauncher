using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plugin.BedrockBoot.Views.Pages.MainSubPage;

namespace Plugin.BedrockBoot.Views.Pages;

public partial class MainHomePage : UserControl
{
    public bool IsEdit = false;

    public MainHomePage()
    {
        InitializeComponent();
        this.Frame.NavigateTo(new HomePage());

        IsEdit = true;
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            var tag = this.Naviga.SelectedIndex;

            switch (tag)
            {
                case 0:
                    this.Frame.NavigateTo(new HomePage());
                    break;
                case 1:
                    this.Frame.NavigateTo(new ManagerPage());
                    break;
                case 2:
                    this.Frame.NavigateTo(new DownloadPage());
                    break;
                case 3:
                    this.Frame.NavigateTo(new SettingsPage());
                    break;
            }
        }
    }
}