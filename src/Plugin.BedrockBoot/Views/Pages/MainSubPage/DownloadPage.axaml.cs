using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using BedrockLauncher.Core.JsonHandle;
using BedrockLauncher.Core.Network;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls;

namespace Plugin.BedrockBoot.Views.Pages.MainSubPage;

public partial class DownloadPage : UserControl
{
    public DownloadPage()
    {
        InitializeComponent();

        Task.Run(() =>
        {
            Console.WriteLine("正在加载基岩版版本列表...");
            var lst = VersionHelper.GetVersions(
                "https://raw.gitcode.com/gcw_lJgzYtGB/-MineCraft-Bedrock-Download-SU/raw/main/bedrock.json");
            Console.WriteLine("基岩版版本列表加载完成");
            
            Console.WriteLine("开始序列化");
            
            // 预处理：为每个项预先计算 Version 对象
            var versionCache = new List<(VersionInformation item, Version? version)>();
    
            foreach (var item in lst)
            {
                if (string.IsNullOrEmpty(item.ID)) continue;
                if (item.Variations.Count <= 0) continue;

                bool isCon = false;

                foreach (var v in item.Variations)
                {
                    if(v.UpdateIds.Count <= 0) isCon = true;
                }

                if (isCon) continue;
        
                Version? version = null;
                try
                {
                    version = new Version(item.ID);
                }
                catch
                {
                    // 忽略无效的版本号
                }
                versionCache.Add((item, version));
            }

            // 使用缓存的 Version 对象进行排序
            versionCache.Sort((x, y) =>
            {
                // 两个都有有效版本号
                if (x.version != null && y.version != null)
                {
                    return y.version.CompareTo(x.version); // 降序
                }
        
                // 只有一个有有效版本号，有效版本号排在前面
                if (x.version != null) return -1;
                if (y.version != null) return 1;
        
                // 两个都没有有效版本号，按原始字符串排序
                return string.Compare(y.item.ID, x.item.ID, StringComparison.Ordinal);
            });

            // 提取排序后的结果
            lst = versionCache.Select(x => x.item).ToList();
            
            Console.WriteLine("序列化完成");
            if (lst != null && lst.Count > 0)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    Console.WriteLine("开始动态修改 UI");
                    lst.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.ID) && x.Type == "Release")
                        {
                            var item = new SettingCard()
                            {
                                Header = x.ID,
                                Description = string.Join(", ",new string?[]
                                {
                                    x.Type,
                                    x.Date
                                }),
                                IsClickable = true,
                                Margin = new Thickness(5,0,5,15),
                            };
                            
                            ItemsPanel.Children.Add(item);
                        }
                    });

                    LoadingRing.IsVisible = false;
                    ItemsPanel.IsVisible = true;
                    Console.WriteLine("UI 修改完毕");
                });
            }
        });
    }
}