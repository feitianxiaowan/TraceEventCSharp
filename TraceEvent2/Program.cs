using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace TraceEvent2
{
    class Program
    {
        public static TextWriter Out = Console.Out;

        static void Main(string[] args)
        {
            //TraceEventSamples.AllSamples.Run();

            //DynamicParserTest();
            RegisterParserTest();

            return;
        }

        static void RegisterParserTestOnline()
        {
            var monitoringTimeSec = 1000;

            if (Environment.OSVersion.Version.Major * 10 + Environment.OSVersion.Version.Minor < 62)
            {
                Out.WriteLine("This demo only works on Win8 / Win 2012 an above)");
                return;
            }

            Out.WriteLine("******************** KernelAndClrMonitor DEMO (Win 8) ********************");
            Out.WriteLine("Printing both Kernel and CLR (user mode) events simultaneously");
            Out.WriteLine("The monitor will run for a maximum of {0} seconds", monitoringTimeSec);
            Out.WriteLine("Press Ctrl-C to stop monitoring early.");
            Out.WriteLine();
            Out.WriteLine("Start a .NET program to see some events!");
            Out.WriteLine();
            if (TraceEventSession.IsElevated() != true)
            {
                Out.WriteLine("Must be elevated (Admin) to run this program.");
                return;
            }

            TraceEventSession session = null;

            // Set up Ctrl-C to stop both user mode and kernel mode sessions
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs cancelArgs) =>
            {
                if (session != null)
                    session.Dispose();
                cancelArgs.Cancel = true;
            };

            // Set up a timer to stop processing after monitoringTimeSec 
            var timer = new Timer(delegate (object state)
            {
                Out.WriteLine("Stopped Monitoring after {0} sec", monitoringTimeSec);
                if (session != null)
                    session.Dispose();
            }, null, monitoringTimeSec * 1000, Timeout.Infinite);

            // Create the new session to receive the events.  
            // Because we are on Win 8 this single session can handle both kernel and non-kernel providers.  
            using (session = new TraceEventSession("MonitorKernelAndClrEventsSession"))
            {
                // Enable the events we care about for the kernel
                // For this instant the session will buffer any incoming events.  
                // Enabling kernel events must be done before anything else.   
                // This will fail on Win7.  
                Out.WriteLine("Enabling Image load, Process and Thread events.");
                session.EnableKernelProvider(
                    KernelTraceEventParser.Keywords.ImageLoad |
                    KernelTraceEventParser.Keywords.Process |
                    KernelTraceEventParser.Keywords.Thread);

                // Subscribe the events of interest.   In this case we just print all events.  
                session.Source.Kernel.All += Print;

                Out.WriteLine("Enabling CLR GC and Exception events.");
                // Enable the events we care about for the CLR (in the user session).
                // unlike the kernel session, you can call EnableProvider on other things too.  
                // For this instant the ;session will buffer any incoming events.  
                //session.EnableProvider(
                //    ClrTraceEventParser.ProviderGuid,
                //    TraceEventLevel.Informational,
                //    (ulong)(ClrTraceEventParser.Keywords.GC | ClrTraceEventParser.Keywords.Exception));

                session.Source.Clr.All += Print;
#if DEBUG
                // in debug builds it is useful to see any unhandled events because they could be bugs. 
                session.Source.UnhandledEvents += Print;
#endif
                // process events until Ctrl-C is pressed or timeout expires
                Out.WriteLine("Waiting for Events.");
                session.Source.Process();
            }

            timer.Dispose();    // Turn off the timer.  
        }

        static void Print(TraceEvent data)
        {
            // There are a lot of data collection start on entry that I don't want to see (but often they are quite handy
            if (data.Opcode == TraceEventOpcode.DataCollectionStart)
                return;

            Out.WriteLine(data.ToString());
            if (data is UnhandledTraceEvent)
                Out.WriteLine(data.Dump());
        }

        static void DynamicParserTest()
        {
            var monitoringTimeSec = 500;

            if (!(TraceEventSession.IsElevated() ?? false))
            {
                Out.WriteLine("To turn on ETW events you need to be Administrator, please run from an Admin process.");
            }

            var sessionName = "SimpleMontitorSession";

            using (var session = new TraceEventSession(sessionName))
            { 
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { session.Dispose(); };

                var firstEventTimeMSec = new Dictionary<int, double>();

                session.Source.Dynamic.All += delegate (TraceEvent data)
                {  
                    var delay = (DateTime.Now - data.TimeStamp).TotalSeconds;
                    Out.WriteLine("GOT Event Delay={0:f1}sec: {1} ", delay, data.ToString());
                };


                var restarted = session.EnableProvider("Microsoft-Windows-Kernel-File");
                if (restarted)      // Generally you don't bother with this warning, but for the demo we do. 
                    Out.WriteLine("The session {0} was already active, it has been restarted.", sessionName);

                // Set up a timer to stop processing after monitoringTimeSec
                var timer = new Timer(delegate (object state)
                {
                    Out.WriteLine("Stopped after {0} sec", monitoringTimeSec);
                    session.Source.StopProcessing();
                }, null, monitoringTimeSec * 1000, Timeout.Infinite);


                Out.WriteLine("**** Start listening for events from the Microsoft-Windows-Kernel-File provider.");
 
                session.Source.Process();

                timer.Dispose();    // Done with the timer.  
                Out.WriteLine();
                Out.WriteLine("Stopping the collection of events.");
            }

        }
    }
}
