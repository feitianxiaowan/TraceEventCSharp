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
            var f = File.Create("powershell_dumpfile.txt");
            f.Close();
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
                String timeStamp = ts.TotalMilliseconds.ToString("F4");
                timeStamp=timeStamp.Replace(".", "");
                timeStamp += "00";
                Out.WriteLine(processId + ";" + data.ThreadID.ToString() + ";" + timeStamp + ";" + sample.Replace("\r\n", ";"));
                TextWriter dataOut = new StreamWriter(new FileStream("powershell_dumpfile.txt", FileMode.Append, FileAccess.Write));
                dataOut.WriteLine(processId + ";" + data.ThreadID.ToString() + ";" + timeStamp + ";" + sample.Replace("\r\n", ";"));
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
