using Avalonia.Controls;
using Avalonia.Interactivity;
using OverrideLauncher.Core.Modules.Entry.DownloadEntry;
using RMCL.Controls.Helpers;

namespace RMCL.Controls.Item
{
    public partial class DownloadGameItem : UserControl
    {
        public Action<string> OnDownload = s => { };

        public DownloadGameItem(VersionManifestEntry.Version version)
        {
            InitializeComponent();
            VersionName.Text = version.Id;
            VersionType.Text = version.Type;
            VersionTime.Text = DateTime.Parse(version.Time).ToString("yyyy/MM/dd HH:mm:ss");

            // 使用简化的图像缓存
            string resourcePath = $"avares://RMCL.Controls/Assets/MinecraftIcons/{version.Type}.png";
            var bitmap = SimpleImageCache.GetOrCreateBitmapFromAsset(resourcePath, 24);

            if (bitmap != null)
            {
                IconImage.Source = bitmap;
            }
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            OnDownload(VersionName.Text);
        }
    }
}