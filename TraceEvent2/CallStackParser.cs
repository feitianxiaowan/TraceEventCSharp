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
    class CallStackParser: TraceLogParser
    {
        private static SymbolPath symbolPath = new SymbolPath(SymbolPath.SymbolPathFromEnvironment).Add(SymbolPath.MicrosoftSymbolServerPath);
        private static TextWriter SymbolLookupMessages = new StringWriter();
        private static SymbolReader symbolReader = new SymbolReader(Out, symbolPath.ToString());

        private static TraceCodeAddress codeAddress;

        public CallStackParser()
        {
            EventParser = CallStackEventParser;
            PreProcess = CallStackPreProcess;
        }

        protected void CallStackPreProcess()
        {
            foreach (var process in traceLog.Processes)
            {
                foreach (var module in process.LoadedModules)
                {
                    //if (!module.ModuleFile.Name.Equals( "gdiplus")) continue;
                    traceLog.CodeAddresses.LookupSymbolsForModule(symbolReader, module.ModuleFile);
                }
            }
        }

        public static void CallStackEventParser(TraceEvent data)
        {
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
