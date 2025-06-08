using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMCL.Base.Entry.Java
{
    public class JavaDetils
    {
        public string Version { get; set; }
        public string JavaPath { get; set; }
        public string JavaWPath { get; set; }
        public string Source { get; set; }

        public string Implementor { get; set; }
        
        public JavaDetils(JavaInformation info)
        {
            Version = info.Version;
            JavaPath = info.Java;
            JavaWPath = info.JavaW;
            Source = info.Java;
            Implementor = info.Implementor;
        }
    }

}
