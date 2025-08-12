using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OverrideLauncher.Core.Modules.Classes.Version;
using RMCL.Base.Entry;
using RMCL.Base.Interface;

namespace RMCL.Core.Views.Pages.ChildFramePage.Game.GameClientSettingSubPages;

public partial class ClientLaunchSetting : ISetting, IUISetting
{
    public ClientLaunchSetting()
    {
        InitializeComponent();
    }

    public VersionParse Version { get; set; }

    public void UpdateUI()
    {
        LaunchSetting.Config = ClientManager.ClientSelfConfig.GetClientConfig(new LaunchClientInfo()
        {
            GameFolder = Version.ClientInstances.GameCatalog,
            GameName = Version.ClientInstances.GameName
        });
        LaunchSetting.OnSave = config => ClientManager.ClientSelfConfig.SaveClientConfig(new LaunchClientInfo()
        {
            GameFolder = Version.ClientInstances.GameCatalog,
            GameName = Version.ClientInstances.GameName
        }, config);
        LaunchSetting.OnLoaded();
    }
}