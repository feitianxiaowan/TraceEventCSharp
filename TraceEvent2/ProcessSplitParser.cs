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

        public ProcessSplitParser()
        {
            PreProcess = ProcessSplitPreProcess;
        }

        protected void ProcessSplitPreProcess()
        {
            foreach (var process in traceLog.Processes)
            {
                processID2Name.Add(process.ProcessID, process.Name);

                if (!outputChannls.ContainsKey(process.ProcessID))
                {
                    outputChannls.Add(process.ProcessID, new StreamWriter(new FileStream(process.Name + process.ProcessID + ".pkinfo", FileMode.OpenOrCreate, FileAccess.ReadWrite)));
                }

                foreach (var data in process.EventsInProcess)
                {
                    outputChannls[process.ProcessID].WriteLine(data.ToString());
                }
            }
        }

    }
}
