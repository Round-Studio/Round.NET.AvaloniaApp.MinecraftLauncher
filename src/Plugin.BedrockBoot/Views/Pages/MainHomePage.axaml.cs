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

        IsEdit = true;
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            var tag = this.Naviga.SelectedIndex;

            switch (tag)
            {
                case 2:
                    this.Frame.NavigateTo(new SettingsPage());
                    break;
            }
        }
    }
}