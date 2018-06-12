using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Gnu.Getopt;
using NDesk.Options;

using Microsoft.Diagnostics;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace TraceEvent2
{
    class Program
    {
        public static TextWriter Out = Console.Out;
        public static TextWriter logOut = Console.Out;
        public static TextWriter dataOut = Console.Out;

        static List<string> logFileList = new List<string>();
        static List<string> providerNameList = new List<string>();

        static void Main(string[] args)
        {
            // Parse CommandLine Arguments
            bool show_help = false;
            var commandLineParser = new OptionSet()
            {
                {"l|logfile=", "The logfile's path.", logFile => logFileList.Add(logFile)},
                {"p|provider=", "The provider's names.", providerName => providerNameList.Add(providerName)},
                {"o|output=", "The output file path.", outputPath => setDataOut(outputPath)},
                {"h|help", "Show help information.", v => show_help = v != null}
            };

            try
            {
                commandLineParser.Parse(args);
            }
            catch
            {
                logOut.WriteLine("Parse command line failed");
                ShowHelp(null);
                return;
            }
            if (show_help) ShowHelp(commandLineParser);

            // Start Parse Events, if there exit
            if(logFileList.Count() != 0)
            {
                ParseLogFile();
            }

#if DEBUG
            Debugger.Break();
#endif
            return;
        }

        private static void ShowHelp(OptionSet p)
        {
            Out.WriteLine("Usage:");
            p.WriteOptionDescriptions(Out);
        }

        private static void setDataOut(string outputPath)
        {
            try
            {
                FileStream fs =  new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                dataOut = new StreamWriter(fs);
            }
            catch
            {
                logOut.WriteLine("Can't open " + outputPath);
            }
        }

        public static void ParseLogFile()
        {
            foreach (var logfile in logFileList)
            {
                try { 
                    using (var source = new ETWTraceEventSource(logfile))
                    {
                        if (source.EventsLost != 0)
                            Out.WriteLine("WARNING: there were {0} lost events", source.EventsLost);

                        // Set up callbacks to 
                        source.Clr.All += Print;
                        source.Kernel.All += Print;

                        source.Process();
                        Out.WriteLine("Done Processing.");
                    }
                }
                catch
                {
                    logOut.WriteLine(logfile + " not found!");
                }
            }
        }

        static void Print(TraceEvent data)
        {
            // There are a lot of data collection start on entry that I don't want to see (but often they are quite handy
            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            dataOut.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                dataOut.WriteLine(data.Dump());
        }
    }
}
