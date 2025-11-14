using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RemoteOnline.Core;
using RMCL.Models.Global;
using RMCL.Views.Control.Item;
using RMCL.Views.Page.Main.MainSubPage;

namespace RMCL.Views.Page.Main.OnlineSubPage;

public partial class OnlineHostChooseGame : UserControl
{
    public bool IsEdit { get; set; } = false;
    private UdpClient? _client;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;

    public OnlineHostChooseGame()
    {
        InitializeComponent();
        UpdateUI();
    }

    public async void UpdateUI()
    {
        IsEdit = false;
        LoadBar.IsIndeterminate = true;
        SelGameBox.Items.Clear();
        RefBtn.IsEnabled = false;
        CreateBtn.IsEnabled = false;

        // 停止之前的监听
        StopListening();

        StartListeningAsync();
    }

    private async Task StartListeningAsync()
    {
        string multicastGroup = "224.0.2.60";
        int multicastPort = 4445;

        _cts = new CancellationTokenSource();

        try
        {
            // 创建UdpClient
            _client = new UdpClient();

            // 关键设置：允许地址重用
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // 绑定到任意地址和指定端口
            _client.Client.Bind(new IPEndPoint(IPAddress.Any, multicastPort));

            // 加入多播组
            _client.JoinMulticastGroup(IPAddress.Parse(multicastGroup));

            Console.WriteLine($@"正在监听多播组 {multicastGroup}:{multicastPort}");
            Console.WriteLine(@"等待接收消息...
");

            // 开始异步监听
            _listenTask = ListenForBroadcastsAsync(_cts.Token);

            // 设置超时，比如监听1秒
            await Task.Delay(1000, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            // 正常取消
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"启动监听时发生错误: {ex.Message}");
        }
        finally
        {
            StopListening();
            IsEdit = true;
            LoadBar.IsIndeterminate = false;
            RefBtn.IsEnabled = true;
        }
    }

    private async Task ListenForBroadcastsAsync(CancellationToken cancellationToken)
    {
        while (_client != null && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 异步接收，不设置超时
                var result = await _client.ReceiveAsync(cancellationToken);

                string message = Encoding.UTF8.GetString(result.Buffer);
                DateTime receiveTime = DateTime.Now;

                Console.WriteLine(
                    $@"[{receiveTime:HH:mm:ss}] 来自 {result.RemoteEndPoint.Address}:{result.RemoteEndPoint.Port}");
                Console.WriteLine($@"消息内容: {message}");
                Console.WriteLine($@"原始字节: {BitConverter.ToString(result.Buffer)}");
                Console.WriteLine($@"消息长度: {result.Buffer.Length} 字节");
                Console.WriteLine(new string('-', 50));

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    var info = ParseMessage(message);
                    if (info.Item1 != 0 && !string.IsNullOrEmpty(info.Item2))
                    {
                        SelGameBox.Items.Add(new OnlineClientChooseItem(info.Item1, info.Item2));
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // 正常取消
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"接收消息时出错: {ex.Message}");
                // 继续监听，不退出循环
                await Task.Delay(100, cancellationToken);
            }
        }
    }

    static (int, string) ParseMessage(string message)
    {
        var motd = "";
        var port = 0;
        try
        {
            Console.WriteLine(@"解析消息内容:");

            // 解析 MOTD 部分
            int motdStart = message.IndexOf("[MOTD]") + 6;
            int motdEnd = message.IndexOf("[/MOTD]");
            if (motdStart >= 6 && motdEnd > motdStart)
            {
                motd = message.Substring(motdStart, motdEnd - motdStart);
                Console.WriteLine($@"  MOTD: {motd}");
            }

            // 解析 AD 部分（端口号）
            int adStart = message.IndexOf("[AD]") + 4;
            int adEnd = message.IndexOf("[/AD]");
            if (adStart >= 4 && adEnd > adStart)
            {
                string portStr = message.Substring(adStart, adEnd - adStart);
                if (int.TryParse(portStr, out port))
                {
                    Console.WriteLine($@"  端口: {port}");
                }
            }

            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"解析消息时出错: {ex.Message}");
        }

        return (port, motd);
    }

    private void StopListening()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        if (_client != null)
        {
            try
            {
                _client.DropMulticastGroup(IPAddress.Parse("224.0.2.60"));
                _client.Close();
                _client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"停止监听时出错: {ex.Message}");
            }

            _client = null;
        }

        _listenTask = null;
    }

    private void RefBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        UpdateUI();
    }

    private void SelGameBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsEdit)
        {
            CreateBtn.IsEnabled = true;
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainOnlinePage.NavigationFrame.NavigateTo(new OnlineNavigation());
    }

    private void CreateBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var service = new OnlineService(Path.Combine(PathsList.OnlinePath, "easytier-core.exe"));
        service.Name = ((OnlineClientChooseItem)SelGameBox.SelectedItem).NameTitle;
        service.CreateRoom(((OnlineClientChooseItem)SelGameBox.SelectedItem).Port);
        MainOnlinePage.NavigationFrame.NavigateTo(new OnlineHost(service));
    }
}