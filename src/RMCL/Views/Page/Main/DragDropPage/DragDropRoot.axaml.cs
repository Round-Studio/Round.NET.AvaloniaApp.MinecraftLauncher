using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.Main.DragDropPage;

public partial class DragDropRoot : UserControl
{
    public List<string> Files { get; set; } = new List<string>();

    public DragDropRoot()
    {
        InitializeComponent();
    }

    public void UpdateUI()
    {
        Files.ForEach(x =>
        {
            ChooseFiles.Items.Add(new ComboBoxItem()
            {
                Content = x
            });
        });
        ChooseFiles.SelectedIndex = 0;
    }

    public DragDropRoot(List<string> files) : this()
    {
        Files = files;
        UpdateUI();
    }
}