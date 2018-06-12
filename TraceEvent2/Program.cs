using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

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

        static int dataCollectTime = 0;

        static string sessionName = "apt_session";
        static string etlFileName = "output.etl";

        static TraceEventSession session = new TraceEventSession(sessionName);

        static void Main(string[] args)
        {
            // Parse CommandLine Arguments
            bool show_help = false;
            var commandLineParser = new OptionSet()
            {
                {"l|logfile=", "The logfile's path.", logFile => logFileList.Add(logFile)},
                {"o|output=", "The output file path.", outputPath => SetDataOut(outputPath)},
                {"p|provider=", "The provider's names.", providerName => providerNameList.Add(providerName)},
                // set up etlFileName
                // set up wether to compress etl file
                {"t|collectTime=", "The time of collect data.", time => dataCollectTime = int.Parse(time) },
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

#if DEBUG
            Debugger.Break();
#endif

            // Start Parse Events, if there exist logfile parameter
            if (logFileList.Count() != 0)
            {
                ParseLogFile();
            }
            // or log file
            else if(providerNameList.Count() != 0)
            {
                Out.WriteLine("Ctrl + c to stop collection!");
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { session.Dispose(); };
                if (dataCollectTime != 0)
                {
                    var timer = new Timer(delegate (object state)
                    {
                        Out.WriteLine("Stopped after {0} sec", dataCollectTime);
                        session.Source.StopProcessing();
                    }, null, dataCollectTime * 1000, Timeout.Infinite);
                }
                CollectLogFile();
            }

            WindUp();
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

        private static void WindUp()
        {
            TraceAnalysis.PrintStatisticInfo();
        }

        private static void SetDataOut(string outputPath)
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
                        source.Clr.All += ProcessData;
                        source.Kernel.All += ProcessData;

                        source.Kernel.ALPCReceiveMessage += ALPCAnalysis.ProcessALPC;
                        source.Kernel.ALPCSendMessage += ALPCAnalysis.ProcessALPC;

                        var symbolParser = new RegisteredTraceEventParser(source);
                        symbolParser.All += ProcessData;

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

        public static void CollectLogFile()
        {
            if (TraceEventSession.IsElevated() != true)
            {
                Out.WriteLine("Must be elevated (Admin) to run this program.");
                Debugger.Break();
                return;
            }
            session.Dispose();
            session = new TraceEventSession("apt_session", etlFileName);

            foreach (var provider in providerNameList)
            {
                session.EnableProvider(provider, TraceEventLevel.Always);
            }
            if (dataCollectTime == 0) Thread.Sleep(int.MaxValue);
            else Thread.Sleep(dataCollectTime * 1000);
        }

        public delegate void ProcessDataDel(TraceEvent data);

        private static void ProcessData(TraceEvent data)
        {
            ProcessDataDel processer = TraceAnalysis.PrintPickupInfo;
            //Print(data);
            //TraceAnalysis.Statistic(data);

            processer(data);
        }

        private static void Print(TraceEvent data)
        {
            // There are a lot of data collection start on entry that I don't want to see (but often they are quite handy
            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            //Out.WriteLine(data.ToXml(new StringBuilder()).ToString());
            dataOut.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                dataOut.WriteLine(data.Dump());
        }


    }
}
