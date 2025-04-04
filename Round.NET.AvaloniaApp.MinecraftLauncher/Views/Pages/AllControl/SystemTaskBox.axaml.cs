﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules;
using Round.NET.AvaloniaApp.MinecraftLauncher.Modules.Message;

namespace Round.NET.AvaloniaApp.MinecraftLauncher.Views.Pages.AllControl;

public partial class SystemTaskBox : UserControl
{
    public SystemTaskBox()
    {
        InitializeComponent();
        Core.SystemTask = this;
        Show();
        Task.Run(() =>
        {
            while (true)
            {
                var time = DateTime.Now.ToString("HH:mm:ss");
                Dispatcher.UIThread.Invoke(()=>TimeBox.Content = time);
                Thread.Sleep(100);
            }
        });
    }

    public void UpdateMessage()
    {
        MessageListBox.Children.Clear();
        Task.Run(() =>
        {
            try
            {
                for (int i = Message.Messages.Count - 1; i >= 0; i--)
                {
                    var e = Message.Messages[i];
                    // 处理 message
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var messagebox = new InfoBar()
                        {
                            Title = e.Title,
                            Message = e.Message,
                            IsOpen = true,
                            Severity = e.Type,
                            Opacity = 0,
                            Margin = new Thickness(5),
                            IsClosable = false
                        };

                        MessageListBox.Children.Add(messagebox);
                    });
                }
            }
            catch
            {
                Dispatcher.UIThread.Invoke(UpdateMessage);
            }
        });
    }
    public void Show()
    {
        if (IsVisible)
        {
            MainPanel.Margin = new Thickness(0,40,-380,0);
            BackGrid.Opacity = 0;
            TimeBox.Margin = new Thickness(-50,50);
            Trip1Box.Margin = new Thickness(-50,160);
            MessageScrollViewer.Margin = new Thickness(-40 - 290, 40, 290, 0);
            Task.Run(() =>
            {
                Thread.Sleep(800);
                Dispatcher.UIThread.Invoke(() => this.IsVisible = false);
            });
        }
        else
        {
            MainPanel.Margin = new Thickness(0,40,-10,0);
            BackGrid.Opacity = 0.6;
            this.IsVisible = true;
            TimeBox.Margin = new Thickness(50);
            Trip1Box.Margin = new Thickness(50,160);
            MessageScrollViewer.Margin = new Thickness(40,40,0,0);
            UpdateMessage();
        }
    }

    private void BackGrid_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Core.SystemTask.Show();
    }
}