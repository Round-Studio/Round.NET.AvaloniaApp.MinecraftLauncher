using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace RMCL.Core.Views.Pages.DialogPage.ClientArchiveSetting;

public partial class Slot : UserControl
{
    private int _count;
    private string _id;

    public Slot()
    {
        InitializeComponent();
    }

    public int Count
    {
        get
        {
            return _count;
        }
        set
        {
            _count = value;
            count.Content = value;
        }
    }

    public string ItemID
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
            if (_id != null)
            {
                try
                {
                    var asset = AssetLoader.Open(
                        new Uri($"avares://RMCL.Core/Assets/Minecraft/items/{_id.Replace("minecraft:", "")}.png"));
                    var bitmap = new Bitmap(asset);
                    Image.Source = bitmap;
                }
                catch
                {
                    var asset = AssetLoader.Open(
                        new Uri($"avares://RMCL.Core/Assets/Minecraft/items/barrier.png"));
                    var bitmap = new Bitmap(asset);
                    Image.Source = bitmap;
                }
            }
        }
    }
}