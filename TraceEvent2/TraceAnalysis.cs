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
        private static StreamWriter alpcOutStream = new StreamWriter(new FileStream("alpc.dot", FileMode.OpenOrCreate, FileAccess.ReadWrite));

        private static HashSet<int> outputProcess = new HashSet<int>();
        private static HashSet<int> targetProcessList = new HashSet<int>();
        private static Dictionary<int, int> messageidToPid = new Dictionary<int, int>();

        static int edge_count = 0;
        public static void ProcessALPCSend(TraceEvent data)
        {
            int messageId = (int)(data.PayloadByName("MessageID"));
            if (messageidToPid.ContainsKey(messageId))
               Console.Out.WriteLine("Conflict messageId");
            else
                messageidToPid.Add(messageId, data.ProcessID);
        }

        public static void ProcessALPCRecieve(TraceEvent data)
        {
            int messageId = (int)(data.PayloadByName("MessageID"));
            if(messageidToPid.ContainsKey(messageId))
            {
                int senderPid = messageidToPid[messageId];
                if (!outputProcess.Contains(senderPid))
                {
                    alpcOutStream.WriteLine("PID_" + senderPid+";");
                    outputProcess.Add(senderPid);
                }
                if (!outputProcess.Contains(data.ProcessID))
                {
                    alpcOutStream.WriteLine("PID_" + data.ProcessID+";");
                    outputProcess.Add(data.ProcessID);
                }

                StringBuilder dotString = new StringBuilder();
                dotString.Append("PID_");
                dotString.Append(senderPid);
                dotString.Append(" -> PID_");
                dotString.Append(data.ProcessID);
                //                dotString.Append(" [label = \"");
                //               dotString.Append(edge_count++);
                //               dotString.Append("\"];");
                dotString.Append(";");
                alpcOutStream.WriteLine(dotString.ToString());

                messageidToPid.Remove(messageId);
            }
            else
            {
                //TODO: discard this event
            }
        }
    }


}
