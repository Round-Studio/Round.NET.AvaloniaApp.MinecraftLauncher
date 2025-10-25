using System;
using System.Threading;
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
using RMCL.Models.Helper;

namespace RMCL.Views.Page;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        this.Loaded += (sender, args) =>
        {
#if DEBUG
            GlobalModels.NoticePanel.AddNotice(new NoticeInfo()
            {
                Message = "当前模式为 Debug 模式，请勿用于发布。",
                Title = "Debug Model",
                NoticeType = NoticeType.Info
            });
            
            var date = CheckVersion.GetLinkerTimestamp();
            var zt = CheckVersion.CheckTimeAndExecute24Hour(date);
            Console.WriteLine(@$"当前模式：Debug 模式");
            Console.WriteLine(@$"当前程序集编译日期：{date}");
            Console.WriteLine(@$"当前版本是否在可用时间段内：{zt}");

            if (zt)
            {
                DialogHost.Show(new DialogInfo()
                {
                    Content =
                        $"当前版本为预览版本，请勿添加到整合包中使用。\n当前版本仅作为测试部分功能，将于 24h 后失效，请抓紧时间进行测试。\n当前可用状态：{zt}",
                    Title = "版本模式提示",
                    CloseButtonText = "开始测试",
                    AccountButton = DialogButtons.CloseButton
                });
            }
            else
            {
                DialogHost.Show(new DialogInfo()
                {
                    Content =
                        $"当前版本为预览版本，请勿添加到整合包中使用。\n当前版本仅作为测试部分功能，将于 24h 后失效，当前已失效。\n当前可用状态：{zt}",
                    Title = "版本模式提示",
                    CloseButtonText = "退出",
                    CloseAction = () =>
                    {
                        Environment.Exit(0);
                    },
                    AccountButton = DialogButtons.CloseButton
                });
            }
#endif
        };
    }
}