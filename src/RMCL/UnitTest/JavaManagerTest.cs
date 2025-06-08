using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMCL.JavaManager;

namespace UnitTest
{
    public class JavaManagerTest
    {
        [Test]
        public void Test()
        {
            var javaDetilsArray = JavaManager.SearchJavaAsync().Result;
        }
    }
}
