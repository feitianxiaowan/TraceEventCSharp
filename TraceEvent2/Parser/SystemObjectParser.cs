using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TraceEvent2.DIFT;

namespace TraceEvent2.Parser
{
    class SystemObjectParser : TraceLogParser
    {
        private static Guid kernelFile = new Guid("EDD08927-9CC4-4E65-B970-C2560FB5C289");
        private static Guid kernelProcess = new Guid("22FB2CD6-0E7B-422B-A0C7-2FAD1FD0E716");
        private static Guid kernelRegistry = new Guid("70EB4F03-C1DE-4F73-A051-33D13D5413BD");
        private static Guid kernelNetwork = new Guid("7DD42A49-5329-4832-8DFD-43D979153A88");
        private static Guid powershell = new Guid("A0C1853B-5C40-4B15-8766-3CF1C58F985A");

        private int eventCount = 0;
        ControlFlowGraph graph = new ControlFlowGraph();

        public SystemObjectParser()
        {
            EventParser = SystemObjectEventParser;
        }

        protected void SystemObjectEventParser(TraceEvent data)
        {
            eventCount += 1;
            if(eventCount % 10000 == 0)
            {
                var fs = new FileStream("graphviz.dot", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                var dataOut = new StreamWriter(fs);
                graph.ToGraphvizString();
                dataOut.WriteLine(graph.ToGraphvizString());
                dataOut.Flush();
                dataOut.Close();
            }

            if(data.ProviderGuid == kernelProcess)
            {
                if(data.TaskName == "ProcessStart")
                {
                    try
                    {
                        graph.NewSubject(data.ProcessID.ToString());
                        graph.NewSubject(data.PayloadStringByName("ProcessID"));
                        graph.NewConnection(data.ProcessID.ToString(), data.PayloadStringByName("ProcessID"), "CREAT_PROCESS");
                    }
                    catch
                    {

                    }
                }
            }
            else if(data.ProviderGuid == kernelFile)
            {
                if(data.TaskName == "Create")
                {
                    try
                    {
                        graph.NewSubject(data.ProcessID.ToString());
                        graph.NewObject(data.PayloadStringByName("FileName").Replace('\\','_'));
                        graph.NewConnection(data.ProcessID.ToString(), data.PayloadStringByName("FileName").Replace('\\', '_'), "OPEN_FILE");
                    }
                    catch
                    {

                    }
                }
            }
            else if (data.ProviderGuid == kernelRegistry)
            {

            }
            else if (data.ProviderGuid == kernelNetwork)
            {

            }
            else if (data.ProviderGuid == powershell)
            {

            }
            else
            {

            }
        }
    }
}
