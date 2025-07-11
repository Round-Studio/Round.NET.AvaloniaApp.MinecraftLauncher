using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using RMCL.Base.Interface;
using RMCL.Controls.View;

namespace RMCL.Core.Views.Pages.ChildFramePage.Game.GameClientSettingSubPages;

public partial class ClientScreenshotSetting : ISetting ,IUISetting
{
    public async void UpdateUI()
    {
        try
        {
            var path = System.IO.Path.Combine(Path, PathsDictionary.PathDictionary.ClientScreenshotsFolderName);
            if (!Directory.Exists(path)) return;

            var files = Directory.EnumerateFiles(path)
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
                    var bitmap = await Task.Run(() => Bitmap.DecodeToWidth(fileStream, 210));
                
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        StackPanel.Children.Add(new ImageReader { ImageSource = bitmap });
                    });
                }
                catch (Exception ex)
                {
                    // 记录错误或忽略
                }
            }
        }
        catch (Exception ex)
        {
            // 处理目录访问错误
        }
    }

    public string Path { get; set; }
    public ClientScreenshotSetting()
    {
        InitializeComponent();
    }
}