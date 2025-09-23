using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using RMCL.Base.Entry.Notice;

namespace RMCL.Views.Control.Notice;

public partial class NoticePanel : UserControl
{
    public static List<NoticeInfo> NoticeInfos = new List<NoticeInfo>();

    public NoticePanel()
    {
        InitializeComponent();
    }

    public void AddNotice(NoticeInfo notice)
    {
        NoticeInfos.Add(notice);
        var noticeBox = new NoticeBox(notice);
        noticeBox.OnClose = box => NoticesPanel.Children.Remove(box);

        // 添加到开头
        NoticesPanel.Children.Insert(0, noticeBox);

        // 如果超过5个，删除最后一个（带淡出动画）
        if (NoticesPanel.Children.Count > 5)
        {
            var oldestNotice = ((NoticeBox)NoticesPanel.Children[NoticesPanel.Children.Count - 1]);
            oldestNotice.CloseThis();
        }
    }
}