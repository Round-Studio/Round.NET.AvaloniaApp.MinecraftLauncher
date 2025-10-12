using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.DialogContent;

public partial class DialogInputOnlineCodeContent : UserControl
{
    public string Code => CodeBox.Text;
    public DialogInputOnlineCodeContent()
    {
        InitializeComponent();
    }
}