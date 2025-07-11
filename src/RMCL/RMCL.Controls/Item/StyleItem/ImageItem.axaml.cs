using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using RMCL.Controls.Helpers;

namespace RMCL.Controls.Item.StyleItem;

public partial class ImageItem : UserControl
{
    public string path { get; set; }
    public Action<string> DeleteCallBack { get; set; } = s => { };
    public ImageItem(string Path)
    {
        InitializeComponent();
        path = Path;

        if (File.Exists(Path))
        {
            // 使用简化的图像缓存
            var bitmap = SimpleImageCache.GetOrCreateBitmapFromFile(Path, 48);
            if (bitmap != null)
            {
                ImageShowBox.Background = new ImageBrush()
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill
                };
            }
        }
    }

    public string GetPath()
    {
        return path;
    }

    private void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        DeleteCallBack(path);
    }
}