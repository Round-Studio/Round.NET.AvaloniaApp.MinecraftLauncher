using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using OnePointUI.Avalonia.Base.Entry;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls.Dialog;
using RMCL.Views.Page.Main.MainSubPage;
using Round.SDK.Entry;
using Round.SDK.Helper;

namespace RMCL.Views.Page.DrawerContent.Setting.Behavior;

public partial class DrawerBehaviorPluginInformationContent : UserControl
{
    public PackConfig PackConfig;
    private bool IsEditMode = false;

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
        IsEditMode = false;

        PluginName.Text = PackConfig.PackName;
        PluginFile.Text = $"文件：{PackConfig.PackFile}";
        PluginAuther.Text = $"作者：{PackConfig.PackAuthor}";
        PluginVersion.Text = $"版本：{PackConfig.PackVersion}";
        if (!string.IsNullOrEmpty(PackConfig.PackDescription))
            PluginDescription.Text = PackConfig.PackDescription;
        if (!string.IsNullOrEmpty(PackConfig.PackIconPath))
            if (File.Exists(PackConfig.PackIconPath))
                PluginIcon.Background = new ImageBrush()
                {
                    Stretch = Stretch.UniformToFill,
                    Source = new Bitmap(PackConfig.PackIconPath)
                };



        if (PackConfig.PackFile.EndsWith(".disable")) PluginEnableSwitch.IsChecked = false;
        else PluginEnableSwitch.IsChecked = true;

        IsEditMode = true;
    }

    private void PluginEnableSwitch_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (IsEditMode)
            {
                var state = (bool)PluginEnableSwitch.IsChecked!;

                if (File.Exists(PackConfig.PackFile))
                    if (state)
                    {
                        if (PackConfig.PackFile.EndsWith(".disable"))
                        {
                            File.Move(PackConfig.PackFile, PackConfig.PackFile.Replace(".disable", ""));
                            PackConfig.PackFile = PackConfig.PackFile.Replace(".disable", "");

                            MainSettingPage.Page.SetReStart();

                            Update();
                        }
                    }
                    else
                    {
                        if (PackConfig.PackFile.EndsWith(".rplck"))
                        {
                            File.Move(PackConfig.PackFile, PackConfig.PackFile.Replace(".rplck", ".rplck.disable"));
                            PackConfig.PackFile = PackConfig.PackFile.Replace(".rplck", ".rplck.disable");

                            MainSettingPage.Page.SetReStart();

                            Update();
                        }
                    }
            }
        }
        catch
        {
        }
    }
}