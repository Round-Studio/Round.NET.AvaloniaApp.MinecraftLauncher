using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnePointUI.Avalonia.Base.Entry;
using RMCL.Properties;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.OtherPage;

public partial class OtherOpenSourceProjects : UserControl
{
    public OtherOpenSourceProjects()
    {
        InitializeComponent();
        MainSettingPage.Page.BreadcrumbBar.SetItems(new List<BreadcrumbItemInfo>()
        {
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.Settings_About,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new OtherAboutUs());
                }
            },
            new BreadcrumbItemInfo()
            {
                ItemName = Resource.OtherAboutUs_ComponentLibrary_OpenSource,
                ItemClickAction = (e) =>
                {
                    MainSettingPage.Page.NavigationTo(new OtherOpenSourceProjects());
                }
            }
        });
        
        var type = typeof(Avalonia.AppBuilder);
        var assembly = type.Assembly;
        var version = assembly.GetName().Version;

        AvaloniaVersion.Text = $"Version {version}";
    }
}