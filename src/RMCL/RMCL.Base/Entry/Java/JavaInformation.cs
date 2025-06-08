using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMCL.Base.Entry.Java
{
    public record class JavaInformation
    {
        public string Version                   { get; init; } = string.Empty;
        
        public required string Java             { get; set; }
        
        public required string JavaW            { get;set; }
        
        public required string Implementor      { get; set; }
    }
    
}
