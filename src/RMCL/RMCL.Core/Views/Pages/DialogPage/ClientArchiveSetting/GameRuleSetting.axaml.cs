using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using FluentAvalonia.FluentIcons;
using fNbt;

namespace RMCL.Core.Views.Pages.DialogPage.ClientArchiveSetting;

public partial class GameRuleSetting : UserControl
{
    private NbtFile _nbt;

    public GameRuleSetting(string filepath, NbtFile nbt)
    {
        InitializeComponent();
        _nbt = nbt;
        NbtCompound root = nbt.RootTag;
        NbtCompound data = root.Get<NbtCompound>("Data");
        foreach (NbtString VARIABLE in data.Get<NbtCompound>("GameRules"))
        {
            list.Items.Add($"{VARIABLE.Name}: {VARIABLE.Value}");
        }
    }

    public void Open()
    {
    }
}