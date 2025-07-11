using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using RMCL.Base.Enum.ButtonStyle;
using RMCL.Base.Interface;
using RMCL.Core.Models.Classes.Manager.UserManager;
using RMCL.Core.Models.Classes.Manager;
using ColorHelper = RMCL.Core.Models.Classes.Manager.StyleManager.ColorHelper;

namespace RMCL.Core.Views.Pages.Main.HomePages;

public partial class HomeQuickChoosePlayer : ISetting, IDisposable
{
    private readonly List<IDisposable> _subscriptions = new();
    private bool _disposed = false;

    public HomeQuickChoosePlayer()
    {
        InitializeComponent();
        UpdateUI();
        SetupHoverEvents();

        this.Loaded += (s, e) =>
        {
            HoverArea.Width = Border1.Bounds.Width + 64; // 只改变宽度
        };
    }

    private void SetupHoverEvents()
    {
        // 使用优化的事件订阅管理器
        var enterSubscription = HoverArea.AddDisposableHandler(
            InputElement.PointerEnteredEvent,
            (sender, e) => ShowContentBox());

        var leaveSubscription = HoverArea.AddDisposableHandler(
            InputElement.PointerExitedEvent,
            (sender, e) => HideContentBox());

        _subscriptions.Add(enterSubscription);
        _subscriptions.Add(leaveSubscription);

        HideContentBox();
    }

    private void ShowContentBox()
    {
        HoverArea.IsVisible = true;
        HoverArea.Width = 230; // 只改变宽度
        HoverArea.Height = 260; // 只改变高度
        BackBorder.Opacity = 1;
        UpdateUI();
    }

    private void HideContentBox()
    {
        HoverArea.Width = Border1.Bounds.Width + 20; // 只改变宽度
        HoverArea.Height = 68; // 只改变高度
        BackBorder.Opacity = 0;
        HoverArea.IsVisible = true;
    }

    public void UpdateUI(bool isus = true)
    {
        IsEdit = false;
        var lst = PlayerManager.Player.Accounts;
        var selectedIndex = PlayerManager.Player.SelectIndex;

        if (Config.Config.MainConfig.ButtonStyle.QuickChoosePlayerButton == OrdinaryButtonStyle.Default)
        {
            // 设置大图标（选中的皮肤）
            if (lst.Count <= 0)
            {
                NullBox.IsVisible = true;
                BigSkinIcon.IsVisible = false;
            }
            else if (lst.Count > 0 && selectedIndex >= 0 && selectedIndex < lst.Count)
            {
                NullBox.IsVisible = false;
                BigSkinIcon.IsVisible = true;
                // 重置所有小图标
                SmallSkinIcon1.IsVisible = false;
                SmallSkinIcon2.IsVisible = false;
                SmallSkinIcon3.IsVisible = false;
                SmallSkinIconGroup.IsVisible = true;
                UserCount.IsVisible = false;
                BigSkinIcon.Background = new ImageBrush()
                {
                    Source = SkinHelper.CropAndScaleBitmapOptimized(SkinHelper.Base64ToBitmap(lst[selectedIndex].Skin),
                        new PixelRect(8, 8, 8, 8), 4),
                    Stretch = Stretch.UniformToFill
                };
            }

            // 设置小图标（未选中的皮肤）
            int smallIconIndex = 0;
            for (var i = 0; i < lst.Count; i++)
            {
                if (i == selectedIndex) continue;

                smallIconIndex++;
                if (smallIconIndex > 3) break; // 最多显示3个小图标

                var skinIcon = smallIconIndex == 1 ? SmallSkinIcon1 :
                    smallIconIndex == 2 ? SmallSkinIcon2 : SmallSkinIcon3;

                if (smallIconIndex == 3 && lst.Count > 3)
                {
                    // 如果有超过3个皮肤，第三个图标显示"+N"
                    skinIcon.Child = new TextBlock()
                    {
                        Text = $"+{lst.Count - 3}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White
                    };
                    skinIcon.Background = Brushes.DodgerBlue;
                }
                else
                {
                    // 正常显示皮肤图标
                    skinIcon.Background = new ImageBrush()
                    {
                        Source = SkinHelper.CropAndScaleBitmapOptimized(SkinHelper.Base64ToBitmap(lst[i].Skin),
                            new PixelRect(8, 8, 8, 8), 2),
                        Stretch = Stretch.UniformToFill
                    };
                }

                skinIcon.IsVisible = true;
            }
        }
        else
        {
            // 重置所有小图标
            SmallSkinIcon1.IsVisible = false;
            SmallSkinIcon2.IsVisible = false;
            SmallSkinIcon3.IsVisible = false;
            SmallSkinIconGroup.IsVisible = false;
            NullBox.IsVisible = false;
            BigSkinIcon.IsVisible = true;
            
            // 设置大图标
            if (lst.Count <= 0)
            {
                NullBox.IsVisible = true;
                BigSkinIcon.IsVisible = false;
            }else if (lst.Count > 0 && selectedIndex >= 0 && selectedIndex < lst.Count)
            {
                BigSkinIcon.Background = new ImageBrush()
                {
                    Source = SkinHelper.CropAndScaleBitmapOptimized(SkinHelper.Base64ToBitmap(lst[selectedIndex].Skin),
                        new PixelRect(8, 8, 8, 8), 4),
                    Stretch = Stretch.UniformToFill
                };
                if (lst.Count > 1)
                {
                    UserCount.IsVisible = true;
                    UserCount.Value = lst.Count - 1;
                }
            }
        }

        if (isus)
        {
            // 更新玩家列表
            try
            {
                PlayerListBox.Items.Clear();
                lst.ForEach(x => { PlayerListBox.Items.Add(new ListBoxItem() { Content = x.Account.UserName }); });
                PlayerListBox.SelectedIndex = selectedIndex;
            }
            catch { }
        }

        IsEdit = true;
    }

    private void PlayerListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            PlayerManager.Player.SelectIndex = PlayerListBox.SelectedIndex;
            PlayerManager.SaveConfig();

            UpdateUI(false);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        // 清理所有事件订阅
        foreach (var subscription in _subscriptions)
        {
            subscription?.Dispose();
        }
        _subscriptions.Clear();
    }
}