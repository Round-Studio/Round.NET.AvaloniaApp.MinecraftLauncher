using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Round.SDK.Entry;
using Round.SDK.Helper;

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
        PluginFile.Text = $"文件：{PackConfig.PackFile}";
        PluginAuther.Text = $"作者：{PackConfig.PackAuthor}";
        PluginVersion.Text = $"版本：{PackConfig.PackVersion}";
        if (!string.IsNullOrEmpty(PackConfig.PackDescription))
            PluginDescription.Text = PackConfig.PackDescription;
        if(!string.IsNullOrEmpty(PackConfig.PackIconPath))
            if (File.Exists(PackConfig.PackIconPath))
                PluginIcon.Background = new ImageBrush()
                {
                    Stretch = Stretch.UniformToFill,
                    Source = new Bitmap(PackConfig.PackIconPath)
                };
    }
}