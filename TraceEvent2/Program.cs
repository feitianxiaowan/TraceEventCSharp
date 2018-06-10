using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Diagnostics.Tracing.Parsers;

namespace TraceEvent2
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceEventSamples.AllSamples.Run();

            //testFunc();
            return;
        }

        static void testFunc()
        {
            Console.Write(RegisteredTraceEventParser.GetManifestForRegisteredProvider("Microsoft-Windows-NDIS-PacketCapture"));
        }
    }
}
