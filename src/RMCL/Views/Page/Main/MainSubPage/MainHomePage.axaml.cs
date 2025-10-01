using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RMCL.Base.Entry.Notice;
using RMCL.Base.Enum.Notice;
using RMCL.Models.Global;

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
    }
}