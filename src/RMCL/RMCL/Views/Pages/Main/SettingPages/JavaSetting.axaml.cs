using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Models.Classes;
using RMCL.Views.Windows.Main.ManageWindows;

namespace RMCL.Views.Pages.Main.SettingPages;

public partial class JavaSetting : UserControl
{
    public bool IsEdit { get; set; } = false;
    public JavaSetting()
    {
        InitializeComponent();
        UpdateUI();
    }

    public void UpdateUI()
    {
        IsEdit = false;
        ChooseDefaultJava.Items.Clear();
        
        JavaManager.JavaManager.JavaRoot.Javas.ForEach(x =>
        {
            ChooseDefaultJava.Items.Add(new ComboBoxItem() { Content = x.JavaWPath });
        });
        ChooseDefaultJava.SelectedIndex = JavaManager.JavaManager.JavaRoot.SelectIndex;

        IsEdit = true;
    }

    private void ChooseDefaultJava_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            JavaManager.JavaManager.JavaRoot.SelectIndex = ChooseDefaultJava.SelectedIndex;
            JavaManager.JavaManager.SaveConfig();
        }
    }

    private void SettingsExpanderItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new ManagerJava().ShowDialog(Core.MainWindow);
    }
}