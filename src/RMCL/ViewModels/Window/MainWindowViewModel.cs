using RMCL.Properties;

namespace RMCL.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string WindowTitle { get; } = Resource.WindowTitle;
}