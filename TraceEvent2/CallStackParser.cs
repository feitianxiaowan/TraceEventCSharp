using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.Diagnostics.Symbols;

namespace TraceEvent2
{
    class CallStackParser
    {
        public static TextWriter Out = Console.Out;
        public static TextWriter dataOut = new StreamWriter(new FileStream("dumpfile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));

        private static SymbolPath symbolPath = new SymbolPath(SymbolPath.SymbolPathFromEnvironment).Add(SymbolPath.MicrosoftSymbolServerPath);
        private static TextWriter SymbolLookupMessages = new StringWriter();
        private static SymbolReader symbolReader = new SymbolReader(Out, symbolPath.ToString());

        private static TraceCodeAddress codeAddress;


        public static void ParseLogFileWithCallStack(List<string> logFileList)
        {
            foreach (var logfile in logFileList)
            {
                var source = new ETWTraceEventSource(logfile);
                if (source.EventsLost != 0)
                    Out.WriteLine("WARNING: there were {0} lost events", source.EventsLost);

                var traceLog = TraceLog.OpenOrConvert(logfile, new TraceLogOptions() { ConversionLog = Out });

                foreach(var process in traceLog.Processes)
                {
                    if (process.ProcessID == 16992)
                    {
                        foreach (var module in process.LoadedModules)
                        {
                            //if (!module.ModuleFile.Name.Equals( "gdiplus")) continue;
                            traceLog.CodeAddresses.LookupSymbolsForModule(symbolReader, module.ModuleFile);
                        }
                    }
                }

                TraceLogEventSource logEventSource = traceLog.Events.GetSource();

                logEventSource.AllEvents += ProcessCallStack;

                logEventSource.Process();
            }
        }

        public static void ProcessCallStack(TraceEvent data)
        {
            if (data.ProcessID != 16992)
                return;

            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            //Out.WriteLine(data.ToXml(new StringBuilder()).ToString());
            dataOut.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                dataOut.WriteLine(data.Dump());

            dataOut.Write(data.EventName + ":");

            var callStack = data.CallStack();
            while (callStack != null)
            {
                //var codeAddress = callStack.CodeAddress;
                //if (codeAddress.Method == null)
                //{


                //    var moduleFile = codeAddress.ModuleFile;
                //    if (moduleFile == null)
                //    {

                //    }
                //    else
                //    {
                //        codeAddress.CodeAddresses.LookupSymbolsForModule(symbolReader, moduleFile);
                //        dataOut.Write(codeAddress.FullMethodName + ",");
                //    }
                //}
                //callStack = callStack.Caller;

                var method = callStack.CodeAddress.Method;
                var module = callStack.CodeAddress.ModuleFile;
                if(method != null)
                {
                    var lineInfo = "";
                    var sourceLocation = callStack.CodeAddress.GetSourceLine(symbolReader);
                    if (sourceLocation != null)
                        lineInfo = string.Format("  AT: {0}({1})", Path.GetFileName(sourceLocation.SourceFile.BuildTimeFilePath), sourceLocation.LineNumber);
                    dataOut.WriteLine("    Method: {0}!{1}{2}", module.Name, method.FullMethodName, lineInfo);
                }
                else if (module != null)
                    dataOut.WriteLine("    Module: {0}!0x{1:x}", module.Name, callStack.CodeAddress.Address);
                else
                    dataOut.WriteLine("    ?!0x{0:x}", callStack.CodeAddress.Address);

                callStack = callStack.Caller;
            }
        }
    }
}
