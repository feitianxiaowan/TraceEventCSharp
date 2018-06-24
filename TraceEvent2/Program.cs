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

        private static List<string> logFileList = new List<string>();
        private static List<string> providerNameList = new List<string>();

        private static int dataCollectTime = 0;

        private static HashSet<int> processWhiteList = new HashSet<int>();
        private static bool processWhiteListFlag = false;

        private static string sessionName = "apt_session";
        private static string etlFileName = "output.etl";

        private static TraceEventSession session = new TraceEventSession(sessionName);

        enum RunningMode : byte
        {
            coOccurenceMatrixMode,
            ALPCAnalysisMode,
            ParseMode,
        }

        static void Main(string[] args)
        {
            // Parse CommandLine Arguments
            bool show_help = false;
            bool real_time = false;
            bool print_manifest = false;

            RunningMode mode = RunningMode.ParseMode;

            var commandLineParser = new OptionSet()
            {
                {"r|realtime", "Start real-time session.",  v => real_time = true},
                {"manifest", "Get Manifest of certain providers.", v => print_manifest = true},

                {"l|logfile=", "The logfile's path.", logFile => logFileList.Add(logFile)},
                {"o|output=", "The output file path.", outputPath => SetDataOut(outputPath)},
                {"p|provider=", "The provider's names.", providerName => providerNameList.Add(providerName)},
                {"providerList=", "The providers names' list file", listName => ReadInProviderList(listName)},
                // set up wether to compress etl file

                {"t|collectTime=", "The time of collect data.", time => dataCollectTime = int.Parse(time) },

                {"processId=", "The process white list.", id => { processWhiteList.Add(Int32.Parse(id)); processWhiteListFlag = true; } },

                {"h|help", "Show help information.", v => show_help = v != null},
                {"m|mode=", @"Choice an running mode. 'c' for CoOccurenceMatrix, 'a' for ALPC analysis, 'p' for parse", m => {switch(m){
                        case "a": mode = RunningMode.ALPCAnalysisMode; break;
                        case "p": mode = RunningMode.ParseMode; break;
                        case "c": mode = RunningMode.coOccurenceMatrixMode; break;
                        default: show_help = true; break;
                    } } }
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
            if (show_help)
            {
                ShowHelp(commandLineParser);
                return;
            }

#if DEBUG
            Debugger.Break();
#endif
            // 选择处理函数
            switch (mode)
            {
                case RunningMode.ParseMode:
                    processer = TraceAnalysis.PrintPickupInfo; break;
                case RunningMode.ALPCAnalysisMode:
                    processer = ALPCAnalysis.ProcessALPC; break;
                case RunningMode.coOccurenceMatrixMode:
                    processer = CoOccurenceMatrix.ProcessCoOccurence; break;
                default: processer = Print; break;
            }

            if (print_manifest)
            {
                PrintManifest();
                return;
            }

            if (real_time)
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
                ParseRealtime();
            }
            // Start Parse Events, if there exist logfile parameter
            else if (logFileList.Count() != 0)
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
            Out.WriteLine("Samples:");
            Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl --processId=4188 --mode=p");
            Out.WriteLine(@"TraceEvent2.exe --providerList=allProviders.txt --manifest");
            Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl --processId=4188 --processId=6464 --processId=644 --processId=284 --mode=c > CoOccurenceMatrix.csv");
        }

        private static void PrintManifest()
        {
            foreach(var provider in providerNameList)
            {
                PrintManifest(provider);
            }
        }

        private static void PrintManifest(string providerName)
        {
            string manifest;
            try
            {
                manifest = RegisteredTraceEventParser.GetManifestForRegisteredProvider(providerName);
            }
            catch
            {
                return;
            }
            FileStream fs = new FileStream(providerName + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            dataOut = new StreamWriter(fs);
            dataOut.Write(manifest);
            dataOut.Close();
            fs.Close();
        }

        private static void PrintAllManifest()
        {
            // list all registered proviers in the system
            
        }

        private static void ReadInProviderList(string listName)
        {
            TextReader dataIn;
            string providerName;
            try
            {
                FileStream fs = new FileStream(listName, FileMode.Open, FileAccess.ReadWrite);
                dataIn = new StreamReader(fs);
            }
            catch
            {
                logOut.WriteLine("Can't open listName file:" + listName);
                return;
            }

            try
            {
                while ((providerName = dataIn.ReadLine()) != null) providerNameList.Add(providerName.Trim());
            }
            catch
            {

            }
        }

        private static void WindUp()
        {
            //TraceAnalysis.PrintStatisticInfo();
            CoOccurenceMatrix.PrintCoOccurenceMatrix();
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

        public static void ParseRealtime()
        {
            if (TraceEventSession.IsElevated() != true)
            {
                Out.WriteLine("Must be elevated (Admin) to run this program.");
                Debugger.Break();
                return;
            }
            session.Dispose();
            session = new TraceEventSession(sessionName);

            session.EnableKernelProvider(KernelTraceEventParser.Keywords.ImageLoad);

            session.Source.Kernel.All += ProcessData;

            //foreach (var provider in providerNameList)
            //{
            //    session.EnableProvider(provider, TraceEventLevel.Always);
            //}

            session.Source.Process();
        }

        public static void ParseLogFile()
        {
            foreach (var logfile in logFileList)
            {
                var source = new ETWTraceEventSource(logfile);
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

        public static void CollectLogFile()
        {
            if (TraceEventSession.IsElevated() != true)
            {
                Out.WriteLine("Must be elevated (Admin) to run this program.");
                Debugger.Break();
                return;
            }
            session.Dispose();
            session = new TraceEventSession(sessionName, etlFileName);

            foreach (var provider in providerNameList)
            {
                session.EnableProvider(provider, TraceEventLevel.Always);
            }
            if (dataCollectTime == 0) Thread.Sleep(int.MaxValue);
            else Thread.Sleep(dataCollectTime * 1000);
        }

        public delegate void ProcessDataDel(TraceEvent data);
        public static ProcessDataDel processer = Print;

        private static void ProcessData(TraceEvent data)
        {
            if (Filter(data))
                return;

            processer(data);
        }

        private static bool Filter(TraceEvent data)
        {
            if (!processWhiteListFlag) return false;
            else if (processWhiteList.Contains(data.ProcessID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void Print(TraceEvent data)
        {
            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            //Out.WriteLine(data.ToXml(new StringBuilder()).ToString());
            dataOut.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                dataOut.WriteLine(data.Dump());
        }
    }
}
