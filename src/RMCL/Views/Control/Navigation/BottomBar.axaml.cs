using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls;
using RMCL.Base.Entry.Navigation;

namespace RMCL.Views.Control.Navigation;

public partial class BottomBar : UserControl
{
    public List<ItemButton> Items { get; set; } = new ();
    public Action<string> OnNavigation { get; set; }
    public BottomBar()
    {
        InitializeComponent();
    }

    public void RegisterItems(BottomBarItemInfo info)
    {
        var newi = new ItemButton()
        {
            ItemGlyph = info.ItemGlyph,
            ItemText = info.ItemText,
            Tag = info.Tag
        };

        var classesName = info.IsSelected ? "NoBorderAccent" : "NoBorder";
        var classesNameText = info.IsSelected ? "Accent" : "";
        newi.Item = new Button()
        {
            Height = 32,
            Classes = { classesName },
            CornerRadius = new  CornerRadius(16),
            Tag = newi.Tag,
            Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(12, 0),
                Children =
                {
                    new FontIcon()
                    {
                        Glyph = newi.ItemGlyph,
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 8, 0)
                    },
                    new TextBlock()
                    {
                        Text = newi.ItemText,
                        FontWeight = FontWeight.Medium,
                        VerticalAlignment = VerticalAlignment.Center,
                        Classes = { classesNameText },
                        Name = "ItemText"
                    }
                }
            }
        };
        newi.Item.Click += (s, e) =>
        {
            var tag = ((Button)s).Tag.ToString();
            
            Items.ForEach(x =>
            {
                var itemText = (TextBlock)((StackPanel)x.Item.Content).Children[1];
                if (x.Tag == tag)
                {
                    x.IsSelected = true;
                    
                    x.Item.Classes.Clear();
                    x.Item.Classes.Add("NoBorderAccent"); 
                    
                    itemText.Classes.Clear();
                    itemText.Classes.Add("Accent");
                }
                else
                {
                    x.IsSelected = false;
                    x.Item.Classes.Clear();
                    x.Item.Classes.Add("NoBorder");
                    
                    itemText.Classes.Clear();
                }
            });
            OnNavigation.Invoke(tag);
        };
        ItemsPanel.Children.Add(newi.Item);
        Items.Add(newi);
    }

    public class ItemButton : BottomBarItemInfo
    {
        public Button Item { get; set; }
    }
}