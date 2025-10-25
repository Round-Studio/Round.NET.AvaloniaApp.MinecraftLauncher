using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
            Message = "欢迎使用 RMCL",
            Title = "Welcome to RMCL",
            NoticeType = NoticeType.Info
        });
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        GlobalModels.MainWindow?.BeginMoveDrag(e);
    }
}