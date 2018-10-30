using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceEvent2
{
    class PrintInfo
    {
        public static string PickupInfo(TraceEvent data)
        {
            StringBuilder pickedinfo = new StringBuilder();

            pickedinfo.Append(string.Format("Time({0});Process({1});Thread({2});Event({3})", data.TimeStampRelativeMSec, data.ProcessID, data.ThreadID, data.EventName));

            // 可以用PayloadIndex预加载；‘；；；
            foreach(string payloadName in data.PayloadNames)
            {
                pickedinfo.Append(string.Format(";{0}({1})", payloadName, data.PayloadStringByName(payloadName)));
            }

            return pickedinfo.ToString();
        }
    }
}
