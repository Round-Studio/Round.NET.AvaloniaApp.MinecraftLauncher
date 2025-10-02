using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Base.Enum;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Base.Entry.Notice;
using RMCL.Base.Enum.Notice;
using RMCL.Models.Global;
using RMCL.Views.Control;

namespace RMCL.Views.Page.Main.MainSubPage;

public partial class MainHomePage : UserControl
{
    public MainHomePage()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        GlobalModels.NoticePanel.AddNotice(new NoticeInfo()
        {
            Message = "欢迎使用 RMCL\naa\naa",
            Title = "Welcome to RMCL",
            NoticeType = NoticeType.Info
        });

        DialogHost.Show(new DialogInfo()
        {
            Title = "Test Dialog",
            Content = "欢迎使用 RMCL\naa\naa",
            CloseButtonText = "OK",
            AccountButton = DialogButtons.CloseButton,
            IsWindow = true
        });
    }
}