using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.Main.OtherPage;

public partial class OtherOpenSourceProjects : UserControl
{
    public OtherOpenSourceProjects()
    {
        InitializeComponent();
        
        var type = typeof(Avalonia.AppBuilder);
        var assembly = type.Assembly;
        var version = assembly.GetName().Version;

        AvaloniaVersion.Text = $"Version {version}";
    }
}