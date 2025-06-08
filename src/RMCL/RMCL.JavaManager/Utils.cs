using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMCL.JavaManager
{
    internal static class Utils
    {
        public static  (int exitCode,string outPut,string errorPut) RunWindows(string drc, string parms)
        {
            Process proc = new();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                Arguments =  parms,
                FileName = drc,
                CreateNoWindow = true
            };
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            proc.StartInfo = startInfo;
            var isStart = proc.Start();
            if (!isStart)
            {
                return (-1,string.Empty,string.Empty);
            }

            var outPut = proc.StandardOutput.ReadToEnd();
            var errorPut = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            
            return (proc.ExitCode,outPut,errorPut);
        }
    }
}
