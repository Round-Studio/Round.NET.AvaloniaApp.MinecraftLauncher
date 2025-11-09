using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnePointUI.Avalonia.Styling.Controls.OnePointControls;
using OverrideLauncher.Core.Base.Entry.Info.Java;
using OverrideLauncher.Core.Classes.Utilities;
using RMCL.Interface;
using RMCL.Models.Global;

namespace RMCL.Views.Page.Main.SetupPage;

public partial class SetupGame : ISetting
{
    public SetupGame()
    {
        InitializeComponent();
    }

    private void AutomaticSearch_OnClick(object? sender, RoutedEventArgs e)
    {
        // 直接启动异步任务，不等待
        _ = SearchAsync();
    }

    private async Task SearchAsync()
    {
        // 在UI线程中更新UI状态
        this.IsEnabled = false;
        AutomaticSearch.Content = new ProgressRing()
        {
            Foreground = Brushes.White,
            Background = Brushes.Transparent,
            RingWidth = 2
        };

        try
        {
            // 在后台线程中执行耗时操作
            var lst = await Task.Run(async () => await JavaUtil.GetJavaListAsync());
            
            // 这里可以对搜索结果进行处理
            // 例如：更新UI显示搜索结果
            ChooseJavaBox.Items.Clear();
            GlobalModels.Config.Data.JavaList = new List<JavaInfo>();
            lst.ForEach(item =>
            {
                ChooseJavaBox.Items.Add(new ComboBoxItem() { Content = item.JavaPath });
                GlobalModels.Config.Data.JavaList.Add(item);
            });
            if (lst.Count >= 1) ChooseJavaBox.SelectedIndex = 0;
            GlobalModels.Config.Save();
        }
        catch (Exception ex)
        {
            // 处理可能的异常
            Console.WriteLine($"搜索Java时出错: {ex.Message}");
        }
        finally
        {
            // 在UI线程中恢复UI状态
            this.IsEnabled = true;
            AutomaticSearch.Content = "自动搜索 Java";
        }
    }
}