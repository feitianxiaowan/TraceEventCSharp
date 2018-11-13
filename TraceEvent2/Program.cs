using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

using Gnu.Getopt;
using NDesk.Options;

using Microsoft.Diagnostics;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.Diagnostics.Symbols;

using TraceEvent2;
using TraceEvent2.DIFT;
using TraceEvent2.Parser;

/*
Out.WriteLine(@"TraceEvent2.exe --providerList=provider_list.txt --callstack // 抓取包含call stack的指定provider的数据，并存储到默认位置(output.etl)");
Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl  --callstack --mode=s // 用p模式的解析函数，解析包含call stacker的output.etl");
Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl --processId=4188 --mode=p // 用p模式的解析函数，解析output.etl中4188进程的event");
Out.WriteLine(@"TraceEvent2.exe --providerList=allProviders.txt --manifest // 获取指定provider的manifest");
Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl --processId=4188 --processId=6464 --processId=644 --processId=284 --mode=c > CoOccurenceMatrix.csv");
*/

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

        private static TraceEventProviderOptions enableOptions = new TraceEventProviderOptions() { StacksEnabled = false };

        private static EventSourceParser parser;


        private static string etlFileName = "output.etl";


        enum RunningMode : byte
        {
            CallStackParser,
            ProcessSplitParser,
            Default,
        }

        static void Main(string[] args)
        {
            // temp_test();

            // Parse CommandLine Arguments
            bool show_help = false;
            bool real_time = false;
            bool print_manifest = false;
            bool enableCallStack = false;

            RunningMode mode = RunningMode.Default;
            Out.WriteLine("");

#if DEBUG
            Debugger.Break();
#endif

            var commandLineParser = new OptionSet()
            {
                // Top level configuration. Decide how we handle the event.
                {"r|realtime", "Start real-time session.",  v => real_time = true},
                {"c|callstack", "Enable all call stack", v => {enableOptions.StacksEnabled = true; enableCallStack = true; } },

                // Parse function configuration. 
                {"m|mode=", @"Choice an running mode. 'c' for CallStackParser,   'p' for ProcessSplitParser", m => {switch(m){
                        case "p": mode = RunningMode.ProcessSplitParser;
                                parser = new ProcessSplitParser();
                                break;
                        case "c": mode = RunningMode.CallStackParser;
                                parser = new CallStackParser();
                                break;
                        case "s": parser = new PowershellParser();
                                break;
                        case "o": parser = new SystemObjectParser();
                                break;
                        case "d":
                        default: mode = RunningMode.Default;
                                parser = new TraceLogParser();
                                //show_help = true; 
                                break;
                    } } },

                // Detail configuration.
                {"l|logfile=", "The logfile's path.", logFile => logFileList.Add(logFile)},
                {"o|output=", "The output file path.", outputPath => SetDataOut(outputPath)},
                {"p|provider=", "The provider's names.", providerName => providerNameList.Add(providerName)},
                {"providerList=", "The providers names' list file", listName => ReadInProviderList(listName)},
                // set up wether to compress etl file

                {"t|collectTime=", "The time of collect data.", time => dataCollectTime = int.Parse(time) },

                {"processId=", "The process white list.", id => { processWhiteList.Add(Int32.Parse(id)); processWhiteListFlag = true; } },

                // Stand alone.
                {"manifest", "Get Manifest of certain providers.", v => print_manifest = true},
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
            if (show_help)
            {
                ShowHelp(commandLineParser);
                return;
            }

#if DEBUG
            Debugger.Break();
#endif

            if (print_manifest)
            {
                PrintManifest();
                return;
            }

            // 选择处理方法
            if (real_time)
            {
                if (TraceEventSession.IsElevated() != true)
                {
                    Out.WriteLine("Must be elevated (Admin) to run this program.");
                    Debugger.Break();
                    return;
                }
                //Out.WriteLine("Ctrl + c to stop collection!");
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { session.Dispose(); };
                if (dataCollectTime != 0)
                {
                    var timer = new Timer(delegate (object state)
                    {
                        Out.WriteLine("Stopped after {0} sec", dataCollectTime);
                        session.Source.StopProcessing();
                    }, null, dataCollectTime * 1000, Timeout.Infinite);
                }

                parser.ParseRealTime(providerNameList);
            }
            else if (providerNameList.Count() != 0)
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
            // Start Parse Events, if there exist logfile parameter
            else if (logFileList.Count() != 0)
            {
                parser.ParseLogFile(logFileList);
                parser.StartProcess();
                parser.Windup();
            }

           

#if DEBUG
            Debugger.Break();
#endif
            return;
        }

        private static void temp_test()
        {
            ControlFlowGraph temp = new ControlFlowGraph();
            temp.test();
        }

        private static void ShowHelp(OptionSet p)
        {
            Out.WriteLine("Usage:");
            p.WriteOptionDescriptions(Out);
            Out.WriteLine("Samples:");
            Out.WriteLine(@"TraceEvent2.exe --providerList=allProviders.txt --callstack // 抓取包含call stack的指定provider的数据，并存储到默认位置(output.etl)");
            Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl  --callstack --mode=s // 用p模式的解析函数，解析包含call stacker的output.etl");
            Out.WriteLine(@"TraceEvent2.exe --logfile=c:\Users\xiaowan\Desktop\output.etl --processId=4188 --mode=p // 用p模式的解析函数，解析output.etl中4188进程的event");
            Out.WriteLine(@"TraceEvent2.exe --providerList=allProviders.txt --manifest // 获取指定provider的manifest");
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
            logOut.WriteLine(providerName);
            FileStream fs = new FileStream(providerName + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            dataOut = new StreamWriter(fs);
            dataOut.Write(manifest);
            dataOut.Close();
            fs.Close();
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


        private static string sessionName = "apt_sessions";
        private static TraceEventSession session = new TraceEventSession(sessionName);

        private static void CollectLogFile()
        {
            if (TraceEventSession.IsElevated() != true)
            {
                Out.WriteLine("Must be elevated (Admin) to run this program.");
                Debugger.Break();
                return;
            }
            session.Dispose();
            session = new TraceEventSession(sessionName, etlFileName);

            if(enableOptions.StacksEnabled)
            session.EnableKernelProvider(KernelTraceEventParser.Keywords.All, KernelTraceEventParser.Keywords.All); // Enable all call stack for Kernel Provider.

            //session.EnableKernelProvider(KernelTraceEventParser.Keywords.ImageLoad | KernelTraceEventParser.Keywords.Process | KernelTraceEventParser.Keywords.Thread );

            foreach (var provider in providerNameList)
            {
                try
                {
                    session.EnableProvider(provider, TraceEventLevel.Verbose, ulong.MaxValue, enableOptions);
                }
                catch
                {
                    logOut.WriteLine(provider);
                }
            }
            if (dataCollectTime == 0) Thread.Sleep(int.MaxValue);
            else Thread.Sleep(dataCollectTime * 1000);
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


    }
}
