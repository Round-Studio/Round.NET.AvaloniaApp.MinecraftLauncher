using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace RMCL.Controls.Button;

public partial class TaskButton : Avalonia.Controls.Button
{
    public TaskButton()
    {
        InitializeComponent();
        UpdateTaskStatus(0);
    }

    public void UpdateTaskStatus(int num)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (num <= 0)
            {
                StatusDisplay.Text = "";
                TextBored.IsVisible = false;
                Width = 32;
            }
            else
            {
                StatusDisplay.Text = $"当前有{num}个任务正在运行";
                TextBored.IsVisible = true;
                Width = Double.NaN;
            }
        });
    }
}