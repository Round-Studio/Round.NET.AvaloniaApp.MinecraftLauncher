using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using OverrideLauncher.Core.Modules.Classes.Version;
using RMCL.Controls.Helpers;

namespace RMCL.Controls.Item;

public partial class ManagerGameItem : UserControl
{
    public Action<VersionParse> OnLaunch = s => { };
    public Action<VersionParse> OnSetting = s => { };
    private VersionParse _versionParse;
    public ManagerGameItem(VersionParse versionInfo)
    {
        _versionParse = versionInfo;
        InitializeComponent();

        VersionName.Text = versionInfo.ClientInstances.GameName;
        VersionType.Text = versionInfo.GameJson.Type;
        try
        {
            VersionTime.Text = DateTime.Parse(versionInfo.GameJson.Time).ToString("yyyy/MM/dd HH:mm:ss");
        }catch{ }

        var type = !string.IsNullOrEmpty(versionInfo.GameJson.Type)
            ? versionInfo.GameJson.Type
            : $"error";

        if (type == "error")
        {
            ErrorVersion.IsVisible = true;
            RightBtnBox.IsVisible = false;
        }
        
        // 使用简化的图像缓存
        var assetPath = $"avares://RMCL.Controls/Assets/MinecraftIcons/{type}.png";
        var bitmap = SimpleImageCache.GetOrCreateBitmapFromAsset(assetPath, 24);

        if (bitmap != null)
        {
            IconImage.Source = bitmap;
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        OnLaunch(_versionParse);
    }

    private void SettingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        OnSetting(_versionParse);
    }
}