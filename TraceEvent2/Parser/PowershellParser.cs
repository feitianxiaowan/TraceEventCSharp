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
        protected MalPowershellScriptDetector detector = new MalPowershellScriptDetector();

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
                string sample = data.PayloadByName("ScriptBlockText").ToString();
                string processId = data.ProcessID.ToString();
                Out.WriteLine(processId + ";" + sample.Replace("\r\n",";"));
                //string result = detector.Match(sample);
                //if (result == null)
                //    return;
                //Out.WriteLine("Someting wrong here!");
                //Out.WriteLine(result);

                //Out.WriteLine(data.FormattedMessage);
            }


        }


    }
}
