using Plugin.BedrockBoot.Views.Pages;
using Round.SDK.Entry.RMCL;
using Round.SDK.Plugin.RMCL;
using Round.SDK.Plugin.RMCL.Register;

namespace Plugin.BedrockBoot;

public class Plugin : IPluginRMCL
{
    public string Name { get; set; } = "BedrockBoot Plugin";
    public string Description { get; set; } = "BedrockBoot For RMCL";
    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = "Dime";

    public void Initialize()
    {
        Console.WriteLine("欢迎使用由 Dime 开发的 BedrockBoot For RMCL 插件！");

        if (OperatingSystem.IsWindows())
        {
            RegisterService.RegisterBottomBarItem(new BottomBarItemInfo()
            {
                ItemGlyph = "\uE74C",
                Tag = "BedrockBoot",
                ItemText = "BedrockBoot",
                PageType = typeof(MainHomePage)
            });
        }
        else
        {
            Console.WriteLine("当前非 Windows 系统无法使用 BedrockBoot 插件！");
        }
    }
}