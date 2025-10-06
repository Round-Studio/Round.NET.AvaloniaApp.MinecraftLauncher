using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;

namespace RMCL.Views.Control;

public partial class StyleBox : UserControl
{
    public StyleBox()
    {
        InitializeComponent();
        UpdateBack();
    }

    public void UpdateBack()
    {
        RightImage.IsVisible = false;
        LeftImage.IsVisible = false;
        BackgroundBox.IsVisible = false;
        
        var name = BackMaterialHelper.GetStringName(GlobalModels.Config.Data.StyleConfig.BackMaterialType);

        if (!string.IsNullOrEmpty(name))
        {
            if (GlobalModels.Config != null)
            {
                var uri = new Uri($"avares://RMCL/Assets/Image/{name}");

                // 2. 使用 AssetLoader.Open 获取流
                using (var stream = AssetLoader.Open(uri))
                {
                    // 3. 将流解码为 Bitmap
                    var bitmap = new Bitmap(stream);
    
                    // 4. 现在你可以将 bitmap 赋值给 Image 控件的 Source 属性
                    RightImage.Source = bitmap;
                    LeftImage.Source = bitmap;
                    
                    RightImage.IsVisible = true;
                    LeftImage.IsVisible = true;
                }
            }
        }

        if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Image)
        {
            BackgroundBox.IsVisible = true;

            using (var originalBitmap = new Bitmap(GlobalModels.Config.Data.StyleConfig.BackgroundImages[
                       GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex]))
            {
                // 直接基于高度120计算宽度
                int newWidth = (int)(120 * ((double)originalBitmap.Size.Width / originalBitmap.Size.Height));

                var resizedBitmap = originalBitmap.CreateScaledBitmap(new PixelSize(newWidth, 120));

                BackgroundBox.Background = new ImageBrush()
                {
                    Stretch = Stretch.UniformToFill,
                    Source = resizedBitmap
                };
            }
        }
    }
}