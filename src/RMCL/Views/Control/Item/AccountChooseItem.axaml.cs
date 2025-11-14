using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LiteSkinViewer2D;
using LiteSkinViewer2D.Extensions;
using OverrideLauncher.Core.Base.Entry.Account;
using OverrideLauncher.Core.Base.Enum.Account;
using RMCL.Properties;
using SkiaSharp;

namespace RMCL.Views.Control.Item;

public partial class AccountChooseItem : UserControl
{
    public static readonly StyledProperty<Account> AccountProperty =
        AvaloniaProperty.Register<AccountChooseItem, Account>(nameof(Account));

    public Account Account
    {
        get => GetValue(AccountProperty);
        set => SetValue(AccountProperty, value);
    }

    // 缓存默认头像，避免重复加载
    private static Bitmap? _defaultHeadIcon;
    private static readonly object _defaultHeadIconLock = new object();

    public AccountChooseItem()
    {
        InitializeComponent();
        AccountProperty.Changed.AddClassHandler<AccountChooseItem>((x, e) => x.OnAccountChanged());
    }

    public AccountChooseItem(Account account) : this()
    {
        Account = account;
    }

    private void OnAccountChanged()
    {
        if (Account == null) return;

        // 异步更新头像，避免阻塞UI线程
        UpdateHeadIconAsync();
        
        // 同步更新其他文本信息
        AccountName.Text = Account.UserName;
        AccountTypeBox.Text = Account.AccountType switch
        { 
            AccountType.Microsoft => Resource.Account_Microsoft,
            AccountType.Offline => Resource.Account_Offline
        };
    }

    private async void UpdateHeadIconAsync()
    {
        try
        {
            var headIcon = await GetHeadIconAsync(Account.SkinData.SkinBase64);
            if (headIcon != null)
            {
                HeadIconImage.Source = headIcon;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"加载头像失败: {ex.Message}");
            // 使用默认头像
            HeadIconImage.Source = await GetDefaultHeadIconAsync();
        }
    }

    private static async Task<Bitmap> GetHeadIconAsync(string? skinBase64 = null)
    {
        if (string.IsNullOrEmpty(skinBase64))
        {
            return await GetDefaultHeadIconAsync();
        }

        try
        {
            // 在后台线程处理图像解码
            return await Task.Run(() =>
            {
                byte[] imageBytes = Convert.FromBase64String(skinBase64);
                using var stream = new MemoryStream(imageBytes);
                var skBitmap = SKBitmap.Decode(stream);
            
                if (skBitmap == null)
                {
                    Console.WriteLine(@"皮肤Base64数据解码失败，使用默认头像");
                    return GetDefaultHeadIconSync();
                }

                return HeadCapturer.Default.Capture(skBitmap).ToBitmap();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"处理自定义头像失败: {ex.Message}");
            return await GetDefaultHeadIconAsync();
        }
    }

    private static Task<Bitmap> GetDefaultHeadIconAsync()
    {
        return Task.Run(GetDefaultHeadIconSync);
    }

    private static Bitmap GetDefaultHeadIconSync()
    {
        // 双检锁确保线程安全
        if (_defaultHeadIcon != null) 
            return _defaultHeadIcon;

        lock (_defaultHeadIconLock)
        {
            if (_defaultHeadIcon != null) 
                return _defaultHeadIcon;

            var uri = new Uri("avares://RMCL/Assets/Image/Skin/Steve.png");
            using var stream = AssetLoader.Open(uri);
            var skBitmap = SKBitmap.Decode(stream);
            
            if (skBitmap == null)
            {
                throw new InvalidOperationException("Failed to decode default skin image.");
            }

            _defaultHeadIcon = HeadCapturer.Default.Capture(skBitmap).ToBitmap();
            return _defaultHeadIcon;
        }
    }
}