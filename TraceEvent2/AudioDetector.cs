using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;


namespace TraceEvent2
{
    class AudioDetector
    {

        public static TextWriter Out = Console.Out;
        public static StreamWriter dataOut = new StreamWriter(new FileStream("audio.dump", FileMode.OpenOrCreate));

        private static HashSet<Guid> activityID = new HashSet<Guid>();

        private static Guid targetGuid = TraceEventProviders.GetProviderGuidByName("Microsoft-Windows-Audio");

        public static void EventReader(TraceEvent data)
        {
            if(data.ProviderGuid == targetGuid)
            {
                dataOut.WriteLine(data.ActivityID);
                dataOut.WriteLine(data.EventName);
                dataOut.WriteLine(data.ToString());
                dataOut.Flush();

                if (data.EventName == "EventID(131)")
                {
                    Out.WriteLine(data.ActivityID);
                    activityID.Add(data.ActivityID);
                }
                    
                if (data.EventName == "EventID(20)")
                {
                    Out.Write("Process ");
                    Out.Write(data.ProcessID);
                    if (activityID.Contains(data.ActivityID))
                    {
                        Out.WriteLine(" is playing Audio!");
                    }
                    else
                    {
                        Out.WriteLine(" is recording Audio!");
                    }
                }

            }
        }

        public static bool ListContains<T>(List<T> source, List<T> search)
        {
            if (search.Count > source.Count)
                return false;

            return Enumerable.Range(0, source.Count - search.Count + 1)
                .Select(a => source.Skip(a).Take(search.Count))
                .Any(a => a.SequenceEqual(search));
        }
    }
}
