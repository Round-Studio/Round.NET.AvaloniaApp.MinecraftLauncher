using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;

namespace RMCL.Core.Views.Pages.ChildFramePage.Game.GameClientSettingSubPages;

public partial class ClientSeniorSetting : UserControl
{
    public ClientSeniorSetting()
    {
        InitializeComponent();
    }

    private void FixClient_OnClick(object? sender, RoutedEventArgs e)
    {
        var s1 = new ContentDialog()
        {
            Content = "修复版本仅支持 18w47b （对应正式版 1.14） 以后的版本。如果您的版本由其他第三方启动器安装，则可能无法正常修复。\n" +
                      "\n" +
                      "免责声明：RMCL 的修复可能会导致此版本永久损坏或丢失，请修复前做好备份重要 存档 & 数据 的措施。\n" +
                      "如因 RMCL 修复版本导致的版本永久损坏或丢失，本启动器及其团队不做负责。\n" +
                      "\n" +
                      "在修复前，您可以选择 手动修复（手动确定版本，不受版本限制） 或者 自动修复（启动器自动检测版本，受版本限制） 方式。\n" +
                      "\n" +
                      "请问您是否需要继续修复？",
            Title = "修复警告",
            CloseButtonText = "取消",
            PrimaryButtonText = "仍要修复",
            DefaultButton = ContentDialogButton.Close
        };
        s1.ShowAsync();
    }
}