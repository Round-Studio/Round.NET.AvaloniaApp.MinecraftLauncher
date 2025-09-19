using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();

        GlobalModels.MainPageContent = this.MainPageContent;
    }
}