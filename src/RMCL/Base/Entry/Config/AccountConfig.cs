using System.Collections.Generic;
using System.Text.Json.Serialization;
using OverrideLauncher.Core.Base.Entry.Account;

namespace RMCL.Base.Entry.Config;

public class AccountConfig
{
    [JsonPropertyName("accounts")] public List<Account> Accounts { get; set; } = new();
}