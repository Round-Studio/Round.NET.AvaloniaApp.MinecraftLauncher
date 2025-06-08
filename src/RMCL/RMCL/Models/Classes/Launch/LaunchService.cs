using System;
using OverrideLauncher.Core.Modules.Classes.Account;
using OverrideLauncher.Core.Modules.Classes.Launch.Client;
using OverrideLauncher.Core.Modules.Classes.Version;
using OverrideLauncher.Core.Modules.Entry.GameEntry;
using OverrideLauncher.Core.Modules.Entry.JavaEntry;
using OverrideLauncher.Core.Modules.Entry.LaunchEntry;
using OverrideLauncher.Core.Modules.Enum.Launch;
using RMCL.Base.Entry;

namespace RMCL.Models.Classes.Launch;

public class LaunchService
{
    public static void Launch(LaunchClientInfo Info)
    {
        var par = new VersionParse(new ClientInstancesInfo()
        {
            GameCatalog = Info.GameFolder,
            GameName = Info.GameName
        });

        ClientRunner run = new ClientRunner(new ClientRunnerInfo()
        {
            Account = new OffineAuthenticator("test").Authenticator(),
            JavaInfo = new JavaInfo()
            {
                JavaPath = Info.Java.JavaWPath,
                Version = Info.Java.Version
            },
            LauncherInfo = "RMCL",
            GameInstances = par,
            WindowInfo = ClientWindowSizeEnum.Fullscreen
        });
        run.LogsOutput = s => Console.WriteLine(s);
        run.Start();
    }
}