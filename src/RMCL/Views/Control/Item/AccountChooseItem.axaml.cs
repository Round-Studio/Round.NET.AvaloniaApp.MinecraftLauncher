using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LiteSkinViewer2D;
using LiteSkinViewer2D.Extensions;
using OverrideLauncher.Core.Base.Entry.Account;
using OverrideLauncher.Core.Base.Enum.Account;
using SkiaSharp;

namespace RMCL.Views.Control.Item;

public partial class AccountChooseItem : UserControl
{
    private Account _account;
    public AccountChooseItem(Account account)
    {
        _account = account;
        InitializeComponent();
        HeadIconImage.Source = GetHeadIcon(account.SkinData.SkinBase64);
        AccountName.Text = account.UserName;
        AccountTypeBox.Text = account.AccountType switch
        { 
            AccountType.Microsoft => "微软正版",
            AccountType.Offline => "离线账户"
        };
    }

    private Bitmap GetHeadIcon(string skinBase64 = null)
    {
        if (string.IsNullOrEmpty(skinBase64))
        {
            var uri = new Uri("avares://RMCL/Assets/Image/Skin/Steve.png");

            using (var stream = AssetLoader.Open(uri))
            {
                // 使用 SKBitmap.Decode 从流解码创建位图
                var skBitmap = SKBitmap.Decode(stream);
            
                if (skBitmap == null)
                {
                    throw new InvalidOperationException("Failed to decode default skin image.");
                }

                // 现在你可以使用 skBitmap 了
                return HeadCapturer.Default.Capture(skBitmap).ToBitmap();
            }
        }
        else
        {
            // 修正：skinBase64 应该是 base64 字符串，需要从 base64 解码
            byte[] imageBytes = Convert.FromBase64String(skinBase64);
            using (var stream = new MemoryStream(imageBytes))
            {
                var skBitmap = SKBitmap.Decode(stream);
            
                if (skBitmap == null)
                {
                    Console.WriteLine($"Skin 用户选项加载失败，皮肤 Base64 无效！玩家 uuid：{_account.UUID}");

                    return GetHeadIcon();
                }

                return HeadCapturer.Default.Capture(skBitmap).ToBitmap();
            }
        }
    }
}