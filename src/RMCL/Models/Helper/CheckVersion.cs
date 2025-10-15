using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RMCL.Models.Helper;

public class CheckVersion
{
    public static bool CheckTimeAndExecute24Hour(DateTime targetTime)
    {
        DateTime currentTime = DateTime.Now;
    
        // 计算时间差
        TimeSpan timeDifference = currentTime - targetTime;
    
        // 检查是否在24小时内
        return timeDifference.TotalHours <= 24 && timeDifference.TotalHours >= 0;
    }

    public static DateTime GetLinkerTimestamp()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var attributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        var buildTimestampAttr = attributes.FirstOrDefault(attr => attr.Key == "BuildTimestamp");
            
        if (buildTimestampAttr != null && 
            DateTime.TryParse(buildTimestampAttr.Value, out var buildTime))
        {
            return buildTime;
        }
            
        return DateTime.MinValue;
    }
    
    public static DateTime GetAssemblyLastWriteTime()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileInfo = new FileInfo(assembly.Location);
        return fileInfo.LastWriteTime;
    }
}