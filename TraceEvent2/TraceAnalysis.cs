using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Diagnostics.Tracing;

namespace TraceEvent2
{
    class TraceAnalysis
    {
        public static TextWriter logOut = Console.Out;

        // statistic
        private static Dictionary<string, Dictionary<int, int>> eventPerProviderPerProcess = new Dictionary<string, Dictionary<int, int>>();

        public static void Statistic(TraceEvent data)
        {
            if (eventPerProviderPerProcess.ContainsKey(data.ProviderName))
            {
                if (eventPerProviderPerProcess[data.ProviderName].ContainsKey(data.ProcessID))
                {
                    eventPerProviderPerProcess[data.ProviderName][data.ProcessID] += 1;
                }
                else
                {
                    eventPerProviderPerProcess[data.ProviderName].Add(data.ProcessID, 1);
                }
            }
            else
            {
                eventPerProviderPerProcess.Add(data.ProviderName, new Dictionary<int, int>());
                eventPerProviderPerProcess[data.ProviderName].Add(data.ProcessID, 1);
            }
        }

        public static void PrintStatisticInfo()
        {
            foreach (var iter in eventPerProviderPerProcess)
            {
                logOut.Write(iter.Key + ",");
                foreach (var iter2 in iter.Value)
                {
                    logOut.Write(iter2.Key + "," + iter2.Value);
                }
                logOut.WriteLine();
            }
        }

        // pick up output
        private static Dictionary<int, TextWriter> outputChannls = new Dictionary<int, TextWriter>();

        public static void PrintPickupInfo(TraceEvent data)
        {
            if (!outputChannls.ContainsKey(data.ProcessID))
            {
                outputChannls.Add(data.ProcessID, new StreamWriter(new FileStream(data.ProcessID + ".pkinfo", FileMode.OpenOrCreate, FileAccess.ReadWrite)));
            }
            outputChannls[data.ProcessID].WriteLine(PickupInfo(data));
        }

        public static string PickupInfo(TraceEvent data)
        {
            StringBuilder info = new StringBuilder();
            info.Append(data.TimeStamp);
            info.Append(data.EventName);

            return info.ToString();
        }
    }

    class ALPCAnalysis
    {
        public static void ProcessALPC(TraceEvent data)
        {
            data.PayloadByName("MessageID");
        }
    }
}
