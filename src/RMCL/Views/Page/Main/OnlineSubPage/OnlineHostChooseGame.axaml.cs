using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RMCL.Views.Page.Main.OnlineSubPage;

public partial class OnlineHostChooseGame : UserControl
{
    public bool IsEdit { get; set; } = false;
    public OnlineHostChooseGame()
    {
        InitializeComponent();

        UpdateUI();
    }

    public void UpdateUI()
    {
        IsEdit = false;
        LoadBar.IsIndeterminate = true;
        SelGameBox.Items.Clear();
        RefBtn.IsEnabled = false;
        
        string multicastGroup = "224.0.2.60";
        int multicastPort = 4445;
        
        // 创建UdpClient并加入多播组
        using (UdpClient client = new UdpClient(new Random().Next(10000, 60000)))
        {
            // 加入多播组
            client.JoinMulticastGroup(IPAddress.Parse(multicastGroup));
            
            Console.WriteLine($"正在监听多播组 {multicastGroup}:{multicastPort}");
            Console.WriteLine("等待接收消息...\n");

            // 设置超时时间，避免无限期等待
            client.Client.ReceiveTimeout = 500;

            try
            {
                while (true)
                {
                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedData = client.Receive(ref remoteEP);
                        
                        string message = Encoding.UTF8.GetString(receivedData);
                        DateTime receiveTime = DateTime.Now;
                        
                        Console.WriteLine($"[{receiveTime:HH:mm:ss}] 来自 {remoteEP.Address}:{remoteEP.Port}");
                        Console.WriteLine($"消息内容: {message}");
                        Console.WriteLine($"原始字节: {BitConverter.ToString(receivedData)}");
                        Console.WriteLine($"消息长度: {receivedData.Length} 字节");
                        Console.WriteLine(new string('-', 50));

                        SelGameBox.Items.Add(new TextBlock() { Text = message });
                        
                        // 解析消息内容
                        ParseMessage(message);
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        // 超时后继续监听
                        Console.WriteLine("等待消息中...");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            finally
            {
                // 离开多播组
                client.DropMulticastGroup(IPAddress.Parse(multicastGroup));
            }
        }

        IsEdit = true;
    }
    static void ParseMessage(string message)
    {
        try
        {
            Console.WriteLine("解析消息内容:");
            
            // 解析 MOTD 部分
            int motdStart = message.IndexOf("[MOTD]") + 6;
            int motdEnd = message.IndexOf("[/MOTD]");
            if (motdStart >= 6 && motdEnd > motdStart)
            {
                string motd = message.Substring(motdStart, motdEnd - motdStart);
                Console.WriteLine($"  MOTD: {motd}");
            }
            
            // 解析 AD 部分（端口号）
            int adStart = message.IndexOf("[AD]") + 4;
            int adEnd = message.IndexOf("[/AD]");
            if (adStart >= 4 && adEnd > adStart)
            {
                string portStr = message.Substring(adStart, adEnd - adStart);
                if (int.TryParse(portStr, out int port))
                {
                    Console.WriteLine($"  端口: {port}");
                }
            }
            
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析消息时出错: {ex.Message}");
        }
    }
}