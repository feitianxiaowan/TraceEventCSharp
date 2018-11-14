using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections.ObjectModel;

namespace PowerShellInteract
{
    class Program
    {
        static void Main(string[] args)
        {

            PowershellInstance ps = new PowershellInstance();



#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif
        }
    }
}
