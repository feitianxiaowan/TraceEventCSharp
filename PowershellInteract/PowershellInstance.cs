using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections.ObjectModel;

namespace PowerShellInteract
{
    class PowershellInstance
    {
        public PowershellInstance()
        {
            PowerShell PowershellInstance = PowerShell.Create();
            PowershellInstance.AddScript("param($param1) $d = get-date; $s = 'test string value'; " +
                "$d; $s; $param1; get-service");
            PowershellInstance.AddParameter("param1", "parameter 1 value!");
            Collection<PSObject> PSOutput = PowershellInstance.Invoke();
            foreach (PSObject outputItem in PSOutput)
            {

            }
        }

    }
}
