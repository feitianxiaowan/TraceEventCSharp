using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using System.Management.Automation;
using System.Collections.ObjectModel;

namespace CSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PowerShell ps = PowerShell.Create();
            

            Collection<PSObject> PSOutput = ps.Invoke();

#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
