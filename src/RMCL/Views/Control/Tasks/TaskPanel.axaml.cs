using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Control.Tasks;

public partial class TaskPanel : UserControl
{
    public bool IsOpen { get; set; } = true;
    public TaskPanel()
    {
        InitializeComponent();
    }

    public bool GetOpenState() => IsOpen;
    public void SetOpenState(bool state) => IsOpen = state;

    public async Task ToggleOpen()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            this.IsVisible = true;
            BackgroundGrid.Opacity = 0.8;
            TasksPanel.Margin = new Thickness(10);
        }
        else
        {
            BackgroundGrid.Opacity = 0;
            TasksPanel.Margin = new Thickness(-220,10,220,10);

            await Task.Delay(380);
            this.IsVisible = false;
        }
    }

    private void BackgroundGrid_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(GetOpenState()) ToggleOpen();
    }
}