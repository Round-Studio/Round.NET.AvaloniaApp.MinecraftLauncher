using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RMCL.Models;
using RMCL.Models.Global;

namespace RMCL.Views.Control;

public partial class StyleBox : UserControl
{
    public StyleBox()
    {
        InitializeComponent();
        UpdatD
            using (var stream = AssetLoader.Open(uri))
            {
                // 3. 将流解码为 Bitmap
                var bitmap = new Bitmap(stream);
    
                // 4. 现在你可以将 bitmap 赋值给 Image 控件的 Source 属性
                RightImage.Source = bitmap;
                LeftImage.Source = bitmap;
            }
        }
    }
}