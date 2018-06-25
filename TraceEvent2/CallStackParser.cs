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

        public static TextWriter dataOut = Console.Out;

        private static SymbolPath symbolPath = new SymbolPath(SymbolPath.SymbolPathFromEnvironment).Add(SymbolPath.MicrosoftSymbolServerPath);
        private static TextWriter SymbolLookupMessages = new StringWriter();
        private static SymbolReader symbolReader = new SymbolReader(SymbolLookupMessages, symbolPath.ToString());

        public static void ProcessCallStack(TraceEvent data)
        {
            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            //Out.WriteLine(data.ToXml(new StringBuilder()).ToString());
            dataOut.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                dataOut.WriteLine(data.Dump());

            var callStack = data.CallStack();
            while (callStack != null)
            {
                var codeAddress = callStack.CodeAddress;
                if (codeAddress.Method == null)
                {
                    var moduleFile = codeAddress.ModuleFile;
                    if (moduleFile == null)
                    {

                    }
                    else
                    {
                        codeAddress.CodeAddresses.LookupSymbolsForModule(symbolReader, moduleFile);
                        dataOut.WriteLine(codeAddress.FullMethodName);
                    }
                }
                callStack = callStack.Caller;
            }

        }
    }
}
