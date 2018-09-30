using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceEvent2
{
    class PowershellParser : TraceLogParser
    {
        protected MalPowershellScriptDetector detector = new MalPowershellScriptDetector();

        DateTime defaultTime = Convert.ToDateTime("1970-1-1 00:00:00");

        public PowershellParser()
        {
            string dumpfilePath = "powershell_dumpfile.txt";
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), dumpfilePath)))
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), dumpfilePath));
            }

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
                TimeSpan ts = data.TimeStamp.Subtract(defaultTime).Duration();

                Out.WriteLine(processId + ";" + data.ThreadID.ToString() + ";" + ts.TotalMilliseconds + "000" + ";" + sample.Replace("\r\n", "#").Replace(";", "#"));
                TextWriter dataOut = new StreamWriter(new FileStream("powershell_dumpfile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
                dataOut.WriteLine(processId + ";" + data.ThreadID.ToString() + ";" + ts.TotalMilliseconds + "000" + ";" + sample.Replace("\r\n", "#").Replace(";", "#"));
                dataOut.Flush();
                dataOut.Close();
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
