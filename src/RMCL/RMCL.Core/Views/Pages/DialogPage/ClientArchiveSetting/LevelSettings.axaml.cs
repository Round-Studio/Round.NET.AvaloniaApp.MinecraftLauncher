using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using fNbt;
using RMCL.Config;

namespace RMCL.Core.Views.Pages.DialogPage.ClientArchiveSetting;

public partial class LevelSettings : UserControl
{
    private GenericSetting genericSetting;
    private GameRuleSetting gameRuleSetting;
    private PlayerSettings playerSettings;

    private void View_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        switch ((int)(((NavigationViewItem)View.SelectedItem).Tag))
        {
            case 1:
                Frame.Content = new GenericSetting(nbtfilepath, _nbt);
                break;

            case 2:
                Frame.Content = new GameRuleSetting(nbtfilepath, _nbt);
                break;

            case 3:
                Frame.Content = new PlayerSettings(nbtfilepath, _nbt);
                break;
        }
    }

    public string nbtfilepath = "";
    public NbtFile _nbt;

    public LevelSettings()
    {
        InitializeComponent();
        View.SelectionChanged += View_SelectionChanged;
    }

    public NbtFile nbt
    {
        get { return _nbt; }
        set
        {
            _nbt = value;
            genericSetting = new GenericSetting(nbtfilepath, _nbt);
            gameRuleSetting = new GameRuleSetting(nbtfilepath, _nbt);
            playerSettings = new PlayerSettings(nbtfilepath, _nbt);
            View.MenuItems.Add(new NavigationViewItem()
            {
                Content = "游戏设置",
                Tag = 1,
                IsSelected = true,
            });
            View.MenuItems.Add(new NavigationViewItem()
            {
                Content = "游戏规则",
                Tag = 2
            });
            View.MenuItems.Add(new NavigationViewItem()
            {
                Content = "玩家设置",
                Tag = 3
            });
        }
    }

    public void SaveToFile()
    {
        genericSetting.data["Player"] = new NbtCompound(playerSettings.player);
        _nbt.RootTag["Data"] = new NbtCompound(genericSetting.data);
        _nbt.SaveToFile(nbtfilepath, NbtCompression.GZip);
    }
}