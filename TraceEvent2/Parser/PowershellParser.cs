using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceEvent2
{
    class PowershellParser : TraceLogParser
    {
        public PowershellParser()
        {
            EventParser = PowershellEventParser;
        }

        protected void PowershellEventParser(TraceEvent data)
        {
            //foreach (string payloadName in data.PayloadNames)
            //{
            //    Out.Write(payloadName + ":");
            //    Out.WriteLine(data.PayloadByName(payloadName));
            //}

            //Out.WriteLine(data.ToString());
            if (data.ID.ToString() == "4104")
            {
                Out.WriteLine(data.PayloadByName("ScriptBlockText"));
                //Out.WriteLine(data.FormattedMessage);
            }


        }


    }
}
