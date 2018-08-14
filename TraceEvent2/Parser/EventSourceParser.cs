using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace TraceEvent2
{
    class EventSourceParser
    {
        protected static TextWriter Out = Console.Out;
        protected static TextWriter logOut = new StreamWriter(new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
        protected static TextWriter dataOut = new StreamWriter(new FileStream("dumpfile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));

        // Delegate eventParser
        protected delegate void EventParserDel(TraceEvent data);
        protected EventParserDel EventParser = Print;

        protected void ProcessData(TraceEvent data)
        {
            EventParser(data);
        }
        private static void Print(TraceEvent data)
        {
            Out.WriteLine(data.ToString());
        }

        // Two Interface
        protected ETWTraceEventSource source;

        public virtual void ParseLogFile(List<string> logFileList)
        {
            foreach (var logfile in logFileList)
            {
                source = new ETWTraceEventSource(logfile);
                if (source.EventsLost != 0)
                    Out.WriteLine("WARNING: there were {0} lost events", source.EventsLost);

                // Set up callbacks to 
                source.AllEvents += ProcessData;
            }
        }

        public void StartProcess()
        {
            source.Process();
            Out.WriteLine("Done Processing.");
        }

        // RealTime
        protected static string sessionName = "apt_sessions";
        protected static TraceEventSession session = new TraceEventSession(sessionName);

        protected static TraceEventProviderOptions enableOptions = new TraceEventProviderOptions() { StacksEnabled = false };

        public void ParseRealTime(List<string> providerNameList)
        {
            session.Dispose();
            session = new TraceEventSession(sessionName);

            //session.EnableKernelProvider(KernelTraceEventParser.Keywords.ImageLoad);

            session.Source.AllEvents += ProcessData;

            foreach (var provider in providerNameList)
            {
                session.EnableProvider(provider, TraceEventLevel.Always, ulong.MaxValue, enableOptions);
            }

            session.Source.Process();
        }
    }

    class TraceLogParser: EventSourceParser
    {
        protected TraceLogEventSource logEventSource;
        protected TraceLog traceLog;

        // Delegate PreProcess
        protected delegate void PreProcessDel();
        protected PreProcessDel PreProcess = NoPreProcess;

        protected static void NoPreProcess()
        {
            
        }

        // Two Interface
        public override void ParseLogFile(List<string> logFileList)
        {
            foreach (var logfile in logFileList)
            {
                traceLog = TraceLog.OpenOrConvert(logfile, new TraceLogOptions() { ConversionLog = logOut });
                if (traceLog.EventsLost != 0)
                    Out.WriteLine("WARNING: there were {0} lost events", traceLog.EventsLost);

                PreProcess();

                logEventSource = traceLog.Events.GetSource();

                logEventSource.AllEvents += ProcessData;
            }
        }

        public void StartProcess()
        {
            logEventSource.Process();
            Out.WriteLine("Done Processing.");
        }

        // Real Time

        private void ParseTraceLogRealtime(List<string> providerNameList)
        {
            session.Dispose();
            session = new TraceEventSession(sessionName);

            session.EnableKernelProvider(KernelTraceEventParser.Keywords.ImageLoad);

            TraceLogEventSource traceLogSource = TraceLog.CreateFromTraceEventSession(session);

            traceLogSource.AllEvents += ProcessData;

            foreach (var provider in providerNameList)
            {
                session.EnableProvider(provider, TraceEventLevel.Always, ulong.MaxValue, enableOptions);
            }

            traceLogSource.Process();
        }
    }
}
