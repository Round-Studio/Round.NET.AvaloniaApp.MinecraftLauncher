using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.DialogContent.Account;

public partial class DialogAddOfflineAccountContent : UserControl
{
    public string? AccountName => AccountNameBox.Text;
    public DialogAddOfflineAccountContent()
    {
        InitializeComponent();
    }
}