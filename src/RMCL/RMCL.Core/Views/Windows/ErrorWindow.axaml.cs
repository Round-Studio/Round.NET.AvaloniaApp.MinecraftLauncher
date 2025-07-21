using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using RMCL.Core.Models.Classes.ExceptionMessage;
using RMCL.Logger;
using RMCL.Modules.Entry;
using RMCL.SystemHelper;

namespace RMCL.Core.Views.Windows;

public partial class ErrorWindow : Window
{
    public ExceptionEntry ExceptionEntry { get; set; } = new();
    public ErrorWindow()
    {
        InitializeComponent();
        TimeShow.Content = DateTime.Now.ToString(); 
    }
    
    public void ShowEx(Exception ex,DateTime time)
    {
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        var sysstr = new SequenceString();
        sysstr.Add(new() { Title = "操作系统版本", Text = Environment.OSVersion.ToString() });
        sysstr.Add(new() { Title = "64位操作系统", Text = Environment.Is64BitOperatingSystem.ToString() });
        sysstr.Add(new() { Title = "系统语言", Text = CultureInfo.InstalledUICulture.Name });
        sysstr.Add(new() { Title = "时区", Text = localTimeZone.DisplayName });
        
        var constr = new SequenceString();
        constr.Add(new() { Title = "背景显示模式", Text = Config.Config.MainConfig.Background.ChooseModel.ToString() });
        constr.Add(new() { Title = "最大下载线程", Text = Config.Config.MainConfig.DownloadThreads.ToString() });

        ExceptionEntry.Exception = ex.Message;
        ExceptionEntry.ExceptionSource = ex.Source;
        ExceptionEntry.StackTrace = ex.StackTrace;
        ExceptionEntry.RecordTime = time;
        ExceptionEntry.SystemLanguage = CultureInfo.InstalledUICulture.Name;
        ExceptionEntry.SystemVersion = Environment.OSVersion.ToString();
        ExceptionEntry.SystemTimeZone = localTimeZone.DisplayName;
        ExceptionEntry.ExceptionType = ExceptionMessage.GetExceptionSeverity(ex);

        ExceptionMessage.LogException(ExceptionEntry);
        ErrorTypeLabel.Content = ex.GetType().Name;
        StackMessage.Text = ex.ToString();
        ErrorMessage.Text = ex.Message;
        ConfigMessage.Text = constr.GetResult();
        SystemMessage.Text = sysstr.GetResult();
    
        var st = new StackTrace(ex, true);
        var frame = st.GetFrame(0); // 获取最顶层的堆栈帧
        
        string fileName = frame?.GetFileName() ?? "未知文件";
        int lineNumber = frame?.GetFileLineNumber() ?? 0;
        string methodName = frame?.GetMethod()?.Name ?? "未知方法";
        
        string errorDetails = $"未处理异常: {ex.GetType().Name}\n" +
                              $"消息: {ex.Message}\n" +
                              $"位置: {fileName} 第 {lineNumber} 行\n" +
                              $"方法: {methodName}\n" +
                              $"完整堆栈:\n{ex.StackTrace}";
        
        Console.WriteLine(errorDetails);
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var con = new ContentDialog()
        {
            Title = "重置",
            Content = "你确定要重置 RMCL 吗？\n如果你重置，则全部配置将消失！",
            CloseButtonText = "取消",
            PrimaryButtonText = "确定",
            DefaultButton = ContentDialogButton.Close
        };
        con.PrimaryButtonClick += (_, _) =>
        {
            try
            {
                Directory.Delete(Path.GetDirectoryName(PathsDictionary.PathDictionary.ConfigPath), true);
                Directory.Delete(Path.GetDirectoryName(PathsDictionary.PathDictionary.SkinFolder), true);
            }catch{ }

            Thread.Sleep(100);
            Environment.Exit(0);
        };
        con.ShowAsync();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void ExportLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "导出日志",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "RMCL 日志文件", Extensions = new List<string> { "log" } },
            },
            DefaultExtension = "log",
            InitialFileName = $"RMCL.Log_{DateTime.Now:yyyyMMdd_HHmmss}.log"
        };
            
        var result = await saveFileDialog.ShowAsync(this);
            
        if (result != null)
        {
            try
            {
                File.Copy(ConsoleRedirector.FileName, result);
            }
            catch (Exception ex)
            {
                    
            }
        }
    }
}