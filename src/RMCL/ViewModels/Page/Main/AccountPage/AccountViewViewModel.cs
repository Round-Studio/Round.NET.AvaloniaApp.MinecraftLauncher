using System.Collections.Generic;
using OverrideLauncher.Core.Base.Entry.Account;
using RMCL.Models.Global;

namespace RMCL.ViewModels.Page.Main.AccountPage;

public partial class AccountViewViewModel
{
    public List<Account> Accounts { get; } = GlobalModels.Config.Data.AccountConfig.Accounts;
}