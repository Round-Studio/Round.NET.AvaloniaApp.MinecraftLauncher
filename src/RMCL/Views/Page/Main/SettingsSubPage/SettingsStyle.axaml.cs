using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HarfBuzzSharp;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SettingsSubPage;

public partial class SettingsStyle : UserControl
{
    public bool IsEditMode { get; set; } = false;
    public SettingsStyle()
    {
        InitializeComponent();
        ChooseLanguage.SelectedIndex = (int)GlobalModels.Config.Data.Language;

        IsEditMode = true;
    }

    private void ChooseLanguage_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEditMode)
        {
            GlobalModels.Config.Data.Language = (LanguageHelper.LanguageType)ChooseLanguage.SelectedIndex;
            GlobalModels.Config.Save();
            
            MainSettingPage.Page.SetReStart();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageHelper.GetStringName(GlobalModels.Config.Data.Language));
            InvalidateVisual();
        }
    }
}