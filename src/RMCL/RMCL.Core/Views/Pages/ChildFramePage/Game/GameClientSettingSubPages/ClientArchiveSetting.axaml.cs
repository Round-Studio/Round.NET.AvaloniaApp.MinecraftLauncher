using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.FluentIcons;
using FluentAvalonia.UI.Controls;
using fNbt;
using Microsoft.VisualBasic.FileIO;
using OverrideLauncher.Core.Modules.Classes.Version;
using RMCL.Base.Entry;
using RMCL.Base.Interface;
using RMCL.Core.Models.Classes.Launch;
using RMCL.Core.Views.Pages.DialogPage.ClientArchiveSetting;
using RMCL.PathsDictionary;
using System.Collections.Generic;
using System.IO;

namespace RMCL.Core.Views.Pages.ChildFramePage.Game.GameClientSettingSubPages;

public partial class ClientArchiveSetting : ISetting, IUISetting
{
    private void UpdateSavesList(params string[] paths)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            list.Items.Clear();
            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                {
                    foreach (var ver in Directory.GetDirectories(path))
                    {
                        try
                        {
                            if (File.Exists($"{ver}\\level.dat"))
                            {
                                var nbt = new NbtFile();
                                nbt.LoadFromFile($"{ver}\\level.dat");
                                NbtCompound root = nbt.RootTag;
                                NbtCompound data = root.Get<NbtCompound>("Data");
                                var versionName = data.Get<NbtString>("LevelName").Value;

                                var delete = new Button()
                                {
                                    Content = new FluentIcon()
                                    {
                                        Icon = FluentIconSymbol.Delete20Regular,
                                        Margin = new Thickness(-10),
                                    },
                                    Margin = new Thickness(5),
                                    Height = 32,
                                    Width = 32
                                };
                                delete.Click += (_, __) =>
                                {
                                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(ver, UIOption.AllDialogs,
                                        RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                                    UpdateSavesList(paths);
                                };
                                var launch = new Button()
                                {
                                    Content = $"启动游戏",
                                    Margin = new Thickness(5),
                                    Height = 32,
                                    Width = 150
                                };
                                launch.Click += (_, __) =>
                                {
                                    LaunchService.LaunchTask(new LaunchClientInfo()
                                    {
                                        GameFolder = System.IO.Path.Combine(Path, "..", ".."),
                                        GameName = System.IO.Path.GetDirectoryName(Path)
                                    });
                                };
                                var sett = new Button()
                                {
                                    Content = new FluentIcon()
                                    {
                                        Icon = FluentIconSymbol.Settings20Regular,
                                        Margin = new Thickness(-10),
                                    },
                                    Margin = new Thickness(5),
                                    Height = 32,
                                    Width = 32
                                };
                                sett.Click += (_, __) =>
                                {
                                    var CurrentLevelSettings = new LevelSettings { nbt = nbt, nbtfilepath = $"{ver}\\level.dat" };
                                    var c = new ContentDialog()
                                    {
                                        Content = CurrentLevelSettings,
                                        Title = "修改存档设置",
                                        PrimaryButtonText = "保存修改并退出",
                                        SecondaryButtonText = "取消",
                                    };
                                    c.PrimaryButtonClick += (_, __) =>
                                    {
                                        CurrentLevelSettings.SaveToFile();
                                        CurrentLevelSettings = null;
                                    };
                                    c.ShowAsync();
                                };
                                var title = new Label()
                                {
                                    Foreground = Brushes.Black,
                                    Content = versionName,
                                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Top,
                                    Margin = new Thickness(5),
                                    FontSize = 17,
                                };
                                list.Items.Add(new ListBoxItem()
                                {
                                    Content = new Grid()
                                    {
                                        Height = 65,
                                        Children =
                                        {
                                            new DockPanel()
                                            {
                                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                                                Children =
                                                {
                                                    new Image()
                                                    {
                                                        Height = 48,
                                                        Width = File.Exists($"{ver}\\icon.png") ? 48 : 0,
                                                        Source = File.Exists($"{ver}\\icon.png")
                                                            ? new Bitmap($"{ver}\\icon.png")
                                                            : null,
                                                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                                                    },
                                                    new Grid()
                                                    {
                                                        Children =
                                                        {
                                                            title,
                                                            new Label()
                                                            {
                                                                Content =
                                                                    $"创建时间：{new DirectoryInfo(ver).CreationTime}，难度：{data.Get<NbtInt>("GameType").Value}",
                                                                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                                                                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                                                                Margin = new Thickness(5),
                                                                FontSize = 15,
                                                                Foreground = Brushes.DarkGray,
                                                            },
                                                        }
                                                    }
                                                }
                                            },

                                            new DockPanel()
                                            {
                                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                                Children =
                                                {
                                                    launch,
                                                    sett,
                                                    delete
                                                }
                                            }
                                        }
                                    },
                                });
                            }
                        }
                        catch
                        {
                        }
                        if (list.Items.Count > 0)
                        {
                            nothingtext.Opacity = 0;
                        }
                        else
                        {
                            nothingtext.Opacity = 1;
                        }
                    }
                }
            }
        });
    }

    public ClientArchiveSetting()
    {
        InitializeComponent();
    }

    public string Path { get; set; }

    public void UpdateUI()
    {
        UpdateSavesList(System.IO.Path.Combine(Path, "saves"));
    }
}