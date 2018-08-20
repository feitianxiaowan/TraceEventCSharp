using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceEvent2
{
    class ProcessSplitParser: TraceLogParser
    {
        private static Dictionary<int, string> processID2Name = new Dictionary<int, string>();
        private static Dictionary<int, TextWriter> outputChannls = new Dictionary<int, TextWriter>();

        //Ngram ngram = new Ngram();
        private static Dictionary<int, Ngram> ngramPerProcess = new Dictionary<int, Ngram>();

        public ProcessSplitParser()
        {
            PreProcess = ProcessSplitPreProcess;
            EventParser = ProcessSplitEventParser;
        }

        protected void ProcessSplitPreProcess()
        {
            foreach (var process in traceLog.Processes)
            {
                if (!processID2Name.ContainsKey(process.ProcessID)) 
                    processID2Name.Add(process.ProcessID, process.Name);

                if (!outputChannls.ContainsKey(process.ProcessID))
                {
                    outputChannls.Add(process.ProcessID, new StreamWriter(new FileStream(process.Name + "_" + process.ProcessID + ".pkinfo", FileMode.OpenOrCreate, FileAccess.ReadWrite)));
                }

                Ngram tempNgram = new Ngram();
                foreach (var data in process.EventsInProcess)
                {
                    if (data.ProviderName == "Windows Kernel") continue;

                    if (data.ProviderGuid == new Guid("2cb15d1d-5fc1-11d2-abe1-00a0c911f518"))
                        continue;
                    if (data.ProviderGuid == new Guid("9e814aad-3204-11d2-9a82-006008a86939"))
                        continue;
                    outputChannls[process.ProcessID].WriteLine(PrintInfo.PickupInfo(data));
                    //tempNgram.Counter(NgramCal(data));
                }
                tempNgram.PrintCounterResult(outputChannls[process.ProcessID]);
                outputChannls[process.ProcessID].Flush();
            }
        }

        protected void ProcessSplitEventParser(TraceEvent data)
        {

        }

        protected string NgramCal(TraceEvent data)
        {
            return data.EventName;
        }
    }
}
