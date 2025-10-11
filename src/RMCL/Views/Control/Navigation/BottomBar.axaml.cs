using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Navigation.SelectBar;
using Round.SDK.Entry.RMCL;

namespace RMCL.Views.Control.Navigation;

public partial class BottomBar : UserControl
{
    private List<BottomBarItemInfo> Items { get; set; } = new List<BottomBarItemInfo>();
    public Action<Type>? OnNavigation { get; set; }
    private bool IsEditing { get; set; } = false;
    public BottomBar()
    {
        InitializeComponent();
    }

    public void RegisterItems(BottomBarItemInfo info)
    {
        Items.Add(info);
        IsEditing = false;

        var item = new SelectBarItem()
        {
            Glyph = info.ItemGlyph,
            ItemText = info.ItemText,
            Tag = info.Tag
        };
        
        ItemsPanel.Items.Insert(0, item);

        if (info.IsSelected)
        {
            ItemsPanel.SelectedItem = item;
        }
        
        IsEditing = true;
    }

    private void ItemsPanel_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditing)
        {
            var tag = ((SelectBarItem)ItemsPanel.SelectedItem).Tag.ToString();

            OnNavigation?.Invoke(Items.Find(x => x.Tag == tag).PageType);
        }
    }
}