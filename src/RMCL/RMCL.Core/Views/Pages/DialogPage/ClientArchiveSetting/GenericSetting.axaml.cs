using System;
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

public partial class GenericSetting : UserControl
{
    private NbtFile _nbt;

    private void Thunder_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            data["thundering"] = new NbtByte("thundering", thunder.IsChecked.Value ? Byte.Parse("1") : Byte.Parse("0"));
        }
        catch { }
    }

    private void Rain_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            data["raining"] = new NbtByte("raining", rain.IsChecked.Value ? Byte.Parse("1") : Byte.Parse("0"));
        }
        catch { }
    }

    private void Time_TextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            data["Time"] = new NbtLong("Time", long.Parse(time.Text));
        }
        catch { }
    }

    private void Diff_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            data["Difficulty"] = new NbtByte("Difficulty", (byte)diff.SelectedIndex);
        }
        catch { }
    }

    private void Lockdiff_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            data["DifficultyLocked"] = new NbtByte("DifficultyLocked", lockdiff.IsChecked.Value ? Byte.Parse("1") : Byte.Parse("0"));
        }
        catch { }
    }

    private void Hardcore_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            data["hardcore"] = new NbtByte("hardcore", hardcore.IsChecked.Value ? Byte.Parse("1") : Byte.Parse("0"));
        }
        catch { }
        ;
    }

    public NbtCompound data;

    public GenericSetting(string filepath, NbtFile nbt)
    {
        InitializeComponent();
        _nbt = nbt;
        NbtCompound root = nbt.RootTag;
        data = root.Get<NbtCompound>("Data");
        hardcore.IsChecked = data.Get<NbtByte>("hardcore").Value == 1;
        hardcore.IsCheckedChanged += Hardcore_IsCheckedChanged;
        lockdiff.IsChecked = data.Get<NbtByte>("DifficultyLocked").Value == 1;
        lockdiff.IsCheckedChanged += Lockdiff_IsCheckedChanged;
        diff.SelectedIndex = data.Get<NbtByte>("Difficulty").Value;
        diff.SelectionChanged += Diff_SelectionChanged;
        time.Text = data.Get<NbtLong>("Time").Value.ToString();
        time.TextChanged += Time_TextChanged;
        rain.IsChecked = data.Get<NbtByte>("raining").Value == 1;
        rain.IsCheckedChanged += Rain_IsCheckedChanged;
        thunder.IsChecked = data.Get<NbtByte>("thundering").Value == 1;
        thunder.IsCheckedChanged += Thunder_IsCheckedChanged;
        seed.Text = data["WorldGenSettings"]["seed"].LongValue.ToString();
        seed.IsReadOnly = true;
    }
}