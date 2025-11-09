using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupRootPage : UserControl
{
    public Dictionary<string, object> PageDictionary = new Dictionary<string, object>()
    {
        ["欢迎"] = new SetupWelcome(),
        ["外观"] = new SetupTheme(),
        ["功能"] = new SetupProgram(),
        ["账户"] = new SetupAccount(),
        ["游戏"] = new SetupGame(),
        ["完成"] = new SetupCompleted()
    };

    public int StepIndex = 0;

    public SetupRootPage()
    {
        InitializeComponent();
        
        PageDictionary.ToList().ForEach(x =>
        {
            TopProgressBar.Items.Add(new TabItem()
            {
                Header = x.Key
            });
        });
        UpdatePage();
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (StepIndex == 0)
        {
            ButtonNext.IsEnabled = true;
            ButtonLast.IsEnabled = false;
        }
        else if (StepIndex >= PageDictionary.Count - 1)
        {
            ButtonNext.IsEnabled = false;
            ButtonLast.IsEnabled = true;
        }
        else
        {
            ButtonNext.IsEnabled = true;
            ButtonLast.IsEnabled = true;
        }
    }

    private void UpdatePage()
    {
        SetupFrame.NavigateTo(PageDictionary.ToList()[StepIndex].Value);
        TopProgressBar.SelectedIndex = StepIndex;
    }

    private void ButtonNext_OnClick(object? sender, RoutedEventArgs e)
    {
        StepIndex++;
        UpdateButton();
        UpdatePage();
    }

    private void ButtonLast_OnClick(object? sender, RoutedEventArgs e)
    {
        StepIndex--;
        UpdateButton();
        UpdatePage();
    }
}