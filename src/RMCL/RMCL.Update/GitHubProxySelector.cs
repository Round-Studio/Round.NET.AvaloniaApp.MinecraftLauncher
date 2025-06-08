using System.Net.NetworkInformation;

namespace RMCL.Update;

public class GitHubProxySelector
{
    private List<string> proxyTemplates;

    public GitHubProxySelector()
    {
        proxyTemplates = new List<string>
        {
            "https://ghproxy.com/{url}",
            "https://gh.ddlc.top/{url}",
            "https://github.moeyy.xyz/{url}",
            "https://gh.idayer.com/{url}",
            "https://ghproxy.net/{url}",
            "https://gh-proxy.com/{url}",       // API
            "https://gh.llkk.cc/{url}"          // API
        };
    }

    public string GetBestProxyUrl()
    {
        var proxyUrls = proxyTemplates;
        var pingResults = new List<(string Url, long PingTime)>();

        using (Ping ping = new Ping())
        {
            foreach (var url in proxyUrls)
            {
                try
                {
                    PingReply reply = ping.Send(url.Replace("https://", "").Replace("http://", "").Split('/')[0]);
                    if (reply.Status == IPStatus.Success)
                    {
                        pingResults.Add((url, reply.RoundtripTime));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ping {url.Replace("https://", "").Replace("http://", "").Split('/')[0]} failed: {ex.Message}");
                }
            }
        }

        if (pingResults.Count == 0)
        {
            throw new InvalidOperationException("没有找到可用的代理。");
        }

        var bestProxy = pingResults[0];
        foreach (var result in pingResults)
        {
            if (result.PingTime < bestProxy.PingTime)
            {
                bestProxy = result;
            }
        }

        return bestProxy.Url;
    }
}