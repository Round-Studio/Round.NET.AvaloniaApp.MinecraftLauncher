using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Round.SDK.Entry;

namespace RMCL.Views.Page.DrawerContent.Setting.Behavior;

public partial class DrawerBehaviorPluginInformationContent : UserControl
{
    public PackConfig PackConfig;
    public DrawerBehaviorPluginInformationContent()
    {
        InitializeComponent();

        if (PackConfig != null)
        {
            Update();
        }
    }

    public DrawerBehaviorPluginInformationContent(PackConfig packConfig) : this()
    {
        PackConfig = packConfig;
        Update();
    }

    public void Update()
    {
        PluginName.Text = PackConfig.PackName;
        if(!string.IsNullOrEmpty(PackConfig.PackIconPath))
            if (File.Exists(PackConfig.PackIconPath))
                PluginIcon.Background = new ImageBrush()
                {
                    Stretch = Stretch.UniformToFill,
                    Source = new Bitmap(PackConfig.PackIconPath)
                };
    }
}