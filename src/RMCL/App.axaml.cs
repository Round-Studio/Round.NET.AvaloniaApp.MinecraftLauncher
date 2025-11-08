using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using OnePointUI.Avalonia.Style.Core;
using RMCL.Base.Entry.Config;
using RMCL.Base.Enum.Style;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Models.Helper;
using RMCL.ViewModels;
using RMCL.Views;
using Round.SDK.Entity;

namespace RMCL;

public partial class App : Application
{
    public override void Initialize()
    {
#if DEBUG
        GlobalModels.Config = new ConfigEntity<ConfigEntry>(PathsList.ConfigPath);
#endif
        
        // Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-hans"); // 简体中文
        Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(LanguageHelper.GetStringName(GlobalModels.Config.Data.Language));
        Console.WriteLine($@"语言配置完毕，当前语言：{LanguageHelper.GetStringName(GlobalModels.Config.Data.Language)}");
        
        ThemeManager.Initialize(this);
        AvaloniaXamlLoader.Load(this);

        LoadColor();
    }
    public static void LoadColor()
    {
        if (GlobalModels.Config.Data.StyleConfig.AccentColorType == AccentColorType.Choose)
            ThemeManager.Instance.SetAccentColor(
                Color.Parse(AccentColor.Colors[GlobalModels.Config.Data.StyleConfig.AccentColorIndex]));

        else if (GlobalModels.Config.Data.StyleConfig.AccentColorType == AccentColorType.Image)
            if (GlobalModels.Config.Data.StyleConfig.StyleType == StyleType.Image)
            {
                if (GlobalModels.Config.Data.StyleConfig.BackgroundImages.Count > 0)
                    if (GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex <=
                        GlobalModels.Config.Data.StyleConfig.BackgroundImages.Count - 1)
                    {
                        var path = GlobalModels.Config.Data.StyleConfig.BackgroundImages[
                            GlobalModels.Config.Data.StyleConfig.BackgroundImageSelectedIndex];
                    
                        var color = ImageColorAnalyzer.GetDominantColors(new Bitmap(path));
                        ThemeManager.Instance.SetAccentColor(color[0]);
                    }
            }
            else
            {
                GlobalModels.Config.Data.StyleConfig.AccentColorType = AccentColorType.Choose;
                GlobalModels.Config.Save();
                
                LoadColor();
            }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}