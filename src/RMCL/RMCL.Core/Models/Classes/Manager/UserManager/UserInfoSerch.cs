using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RMCL.Core.Models.Classes.Manager.UserManager;

public class UserInfoSerch
{
    // 获取玩家的UUID
    public static async Task<string> GetUuidByUsername(string username)
    {
        HttpClient _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync($"https://api.mojang.com/users/profiles/minecraft/{username}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("UUID获取失败");
        }

        var content = await response.Content.ReadAsStringAsync();
        using (JsonDocument jsonDocument = JsonDocument.Parse(content))
        {
            return jsonDocument.RootElement.GetProperty("id").GetString();
        }
    }

    // 获取玩家的皮肤URL
    public static async Task<string> GetSkinUrlByUuid(string uuid)
    {
        HttpClient _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("档案获取失败");
        }

        var content = await response.Content.ReadAsStringAsync();
        using (JsonDocument jsonDocument = JsonDocument.Parse(content))
        {
            // 获取properties数组
            if (!jsonDocument.RootElement.TryGetProperty("properties", out var properties))
            {
                throw new Exception("无效的档案数据: 缺少properties");
            }

            // 查找textures属性
            foreach (var property in properties.EnumerateArray())
            {
                if (property.TryGetProperty("name", out var name) &&
                    name.GetString() == "textures" &&
                    property.TryGetProperty("value", out var value))
                {
                    var texturesBase64 = value.GetString();
                    var decodedBytes = Convert.FromBase64String(texturesBase64);
                    using (JsonDocument texturesDoc = JsonDocument.Parse(decodedBytes))
                    {
                        // 获取textures对象
                        if (!texturesDoc.RootElement.TryGetProperty("textures", out var textures))
                        {
                            throw new Exception("无效的纹理数据: 缺少textures");
                        }

                        // 获取SKIN对象
                        if (textures.TryGetProperty("SKIN", out var skin) &&
                            skin.TryGetProperty("url", out var url))
                        {
                            return url.GetString();
                        }
                    }
                }
            }

            throw new Exception("皮肤URL获取失败: 未找到有效的皮肤数据");
        }
    }
    
    public static async Task<string> GetImageAsBase64Async(string imageUrl)
    {
        try
        {
            // 使用优化的HTTP客户端下载图片数据
            byte[] imageBytes = await NetworkService.SingleInstanceDetector.HttpApiClient.GetByteArrayAsync(imageUrl);

            // 转换为Base64字符串
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting image: {ex.Message}");
            return null;
        }
    }
}