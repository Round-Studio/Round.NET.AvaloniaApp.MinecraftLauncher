using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Control.Item;

public partial class OnlineClientChooseItem : UserControl
{
    public int Port { get; private set; }
    public string NameTitle { get; private set; }

    public OnlineClientChooseItem(int port,string name)
    {
        Port = port;
        NameTitle = name;
        InitializeComponent();

        ClientName.Text = name;
        ClientPort.Text = port.ToString();
    }
}