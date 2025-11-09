using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RMCL.Interface;
using RMCL.Models;
using RMCL.Models.Global;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupWelcome : ISetting
{
    public SetupWelcome()
    {
        InitializeComponent();
        ChooseLanguageBox.SelectedIndex = (int)GlobalModels.Config.Data.Language;
        
        IsEdit = true;
    }

    private void ChooseLanguageBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            GlobalModels.Config.Data.Language = (LanguageHelper.LanguageType)ChooseLanguageBox.SelectedIndex;
            GlobalModels.Config.Save();
            
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageHelper.GetStringName(GlobalModels.Config.Data.Language));
        }
    }
}