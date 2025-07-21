using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace RMCL.Core.Views.Windows;

public partial class TestDrag : Window
{
    public TestDrag()
    {
        InitializeComponent();
        
        AddHandler(DragDrop.DragEnterEvent, DragDropE);
    }

    private void DragDropE(object? sender, DragEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}