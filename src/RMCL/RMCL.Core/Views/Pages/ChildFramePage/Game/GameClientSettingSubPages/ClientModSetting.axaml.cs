using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using RMCL.Base.Interface;
using RMCL.Controls.Item.Client;
using RMCL.PathsDictionary;

namespace RMCL.Core.Views.Pages.ChildFramePage.Game.GameClientSettingSubPages
{
    public partial class ClientModSetting : ISetting, IUISetting
    {
        private void Refresh_OnClick(object? sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        public ClientModSetting()
        {
            InitializeComponent();
            this.Loaded += (s, e) => UpdateUI();
        }

        public string Path { get; set; }

        public void UpdateUI()
        {
            SearchBox.Watermark = $"在 0 个已安装模组中搜索...";
            IsEdit = false;
            ModsList.IsVisible = false;
            NullBox.IsVisible = false;
            ProgressBar.IsVisible = true;

            Console.WriteLine(Path);
            if (!Directory.Exists(Path))
            {
                Console.WriteLine("Directory does not exist");
                NullBox.IsVisible = true;
                ProgressBar.IsVisible = false;
                return;
            }

            var enfiles = Directory.GetFiles(Path, $"*{PathDictionary.ClientModEnablePostfix}");
            var dnfiles = Directory.GetFiles(Path, $"*{PathDictionary.ClientModDisablePostfix}");
            if (enfiles.Length + dnfiles.Length == 0)
            {
                Console.WriteLine("No files found");
                NullBox.IsVisible = true;
                ProgressBar.IsVisible = false;
                return;
            }

            ModsList.Children.Clear();
            Task.Run(() =>
            {
                foreach (var file in enfiles)
                {
                    Console.WriteLine($"Adding file: {file}");
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var it = new ClientModItem();
                        ModsList.Children.Add(it);
                        it.LoadInfoAsync(file); // 调用异步方法

                        SearchBox.Watermark = $"在 {ModsList.Children.Count} 个已安装模组中搜索...";
                    });
                }

                foreach (var file in dnfiles)
                {
                    Console.WriteLine($"Adding file: {file}");
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var it = new ClientModItem();
                        ModsList.Children.Add(it);
                        it.LoadInfoAsync(file); // 调用异步方法

                        SearchBox.Watermark = $"在 {ModsList.Children.Count} 个已安装模组中搜索...";
                    });
                }

                Dispatcher.UIThread.InvokeAsync(() => ProgressBar.IsVisible = false);
            });

            ModsList.IsVisible = true;
            IsEdit = true;
        }
    }
}