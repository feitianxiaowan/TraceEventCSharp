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
        private static HashSet<String> serviceProcessName = new HashSet<string>();
        private static HashSet<String> targetProviderList = new HashSet<string>();
        private static Boolean offline = false;
        static int edge_count = 0;

        private static void init()
        {
            if(offline)
            {
                alpcOutStream.WriteLine("digraph d");
                alpcOutStream.WriteLine("{");
            }
            else
            {
                serviceProcessName.Add("svchost.exe");
                serviceProcessName.Add("services.exe");
                serviceProcessName.Add("csrss.exe");
            }

        }
        public static void ProcessALPCSend(TraceEvent data)
        {
            int messageId = (int)(data.PayloadByName("MessageID"));
            if (messageidToPid.ContainsKey(messageId))
               Console.Out.WriteLine("Conflict messageId");
            else
                messageidToPid.Add(messageId, data.ProcessID);
        }

        private static void getTargetProviderNameList(int pid)
        {
            string cmdline = "logman query providers -pid " + Convert.ToString(pid);
 //           string cmdline = "notepad";
            Console.WriteLine(cmdline);

            System.Diagnostics.Process cmdProcess = new System.Diagnostics.Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.RedirectStandardInput = true;
            cmdProcess.StartInfo.RedirectStandardError = true;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.Start();
            cmdProcess.StandardInput.WriteLine(cmdline + "&exit");
            cmdProcess.StandardInput.AutoFlush = true;
            string output = cmdProcess.StandardOutput.ReadToEnd();

            cmdProcess.WaitForExit();
            cmdProcess.Close();

            string[] cmdOutputLines = output.Split(new string[] {"\r\n"},StringSplitOptions.None);

            Boolean beginFlag = false;
            foreach(string eachLine in cmdOutputLines)
            {
                if (!beginFlag)
                {
                    if (eachLine.Length == 0)
                        continue;
                    if (eachLine[0] == '-')
                        beginFlag = true;
                    continue;
                }
                else
                    if(eachLine.Length == 0)
                {
                        Console.WriteLine(eachLine);
                }
                    else
                {
                    String providerName = eachLine.Split('{')[0];
                    providerName = providerName.Trim();
                    // Console.WriteLine(providerName);
                    targetProviderList.Add(providerName);
                }
            }

        }

        public static void ProcessALPCRecieve(TraceEvent data)
        {
            int messageId = (int)(data.PayloadByName("MessageID"));
            if (messageidToPid.ContainsKey(messageId))
            {
                int senderPid = messageidToPid[messageId];

                if (targetProcessList.Contains(senderPid)
                    || targetProcessList.Contains(data.ProcessID))
                {
                    if (!offline)
                    {
                        if (!targetProcessList.Contains(senderPid)
                            && ProcessAnalysis.PidToProcessName.ContainsKey(senderPid)
                            && !serviceProcessName.Contains(ProcessAnalysis.PidToProcessName[senderPid]))
                            targetProcessList.Add(senderPid);

                        if (!targetProcessList.Contains(data.ProcessID)
                            && ProcessAnalysis.PidToProcessName.ContainsKey(data.ProcessID)
                            && !serviceProcessName.Contains(ProcessAnalysis.PidToProcessName[data.ProcessID]))
                            targetProcessList.Add(data.ProcessID);

                        getTargetProviderNameList(senderPid);
                        getTargetProviderNameList(data.ProcessID);
                    }

                    if (!outputProcess.Contains(senderPid))
                    {
                        if (!ProcessAnalysis.PidToProcessName.ContainsKey(senderPid))
                        {
                            Console.Out.WriteLine("Unhandle process " + senderPid);
                            return;
                        }
                        alpcOutStream.WriteLine("PID_" + senderPid + "[label=\"" + ProcessAnalysis.PidToProcessName[senderPid] + "\"];");
                        outputProcess.Add(senderPid);
                    }

                    if (!outputProcess.Contains(data.ProcessID))
                    {
                        if (!ProcessAnalysis.PidToProcessName.ContainsKey(data.ProcessID))
                        {
                            Console.Out.WriteLine("Unhandle process " + data.ProcessID);
                            return;
                        }
                        alpcOutStream.WriteLine("PID_" + data.ProcessID + "[label=\"" + ProcessAnalysis.PidToProcessName[data.ProcessID] + "\"];");
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
                    alpcOutStream.Flush();
                 }

                messageidToPid.Remove(messageId);
            }
            else
            {
                //TODO: discard this event
            }
        }
    }

    class ProcessAnalysis
    {
        public static Dictionary<int, String> PidToProcessName = new Dictionary<int, string>();

        public static void ProcessProcess(TraceEvent data)
        {
            PidToProcessName[(int)data.PayloadByName("ProcessID")] = data.PayloadStringByName("ImageFileName");
        }
    }

}
