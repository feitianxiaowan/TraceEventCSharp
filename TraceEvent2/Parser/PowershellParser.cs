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
            dataOut = new StreamWriter(new FileStream("powershell_dumpfile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite));
        }

        protected void PowershellEventParser(TraceEvent data)
        {
            //foreach (string payloadName in data.PayloadNames)
            //{
            //    Out.Write(payloadName + ":");
            //    Out.WriteLine(data.PayloadByName(payloadName));
            //}


            if (data.ID.ToString() == "4104")
            {
                string sample = data.PayloadByName("ScriptBlockText").ToString();
                string processId = data.ProcessID.ToString();

                String temp = processId + "#" + data.ThreadID.ToString() + "#" + data.TimeStamp + "#" + sample;
                Out.WriteLine(temp);

                dataOut.WriteLine(temp);
                dataOut.Flush();

                /* Call ASTParser & PowershellInstance here */
                /* ASTParser will return all the part we need */
                /* == "$wc = &('Ne'+'w-'+'obje'+'ct') system.net.webclient; $wc.downloadstring((('C:c'+("{1}{0}" -f 'Users','rf')+'crf'+'Zh'+("{1}{0}" -f'ua','eny')+("{0}{1}" -f'n L','i')+'c'+("{1}{0}"-f'Docu','rf')+("{2}{0}{1}"-f 'tscr','f','men')+'G'+'i'+("{2}{5}{0}{1}{4}{3}" -f'fInvoke-Obfu','sc','t','ionc','at','Hubcr')+("{0}{2}{1}"-f'rfL','E','ICENS')) -rEPlACe'crf',[ChaR]92))" -> 
                 * "&('Ne'+'w-'+'obje'+'ct') system.net.webclient;"
                 * "$wc.downloadstring((('C:c'+("{1}{0}" -f 'Users','rf')+'crf'+'Zh'+("{1}{0}" -f'ua','eny')+("{0}{1}" -f'n L','i')+'c'+("{1}{0}"-f'Docu','rf')+("{2}{0}{1}"-f 'tscr','f','men')+'G'+'i'+("{2}{5}{0}{1}{4}{3}" -f'fInvoke-Obfu','sc','t','ionc','at','Hubcr')+("{0}{2}{1}"-f'rfL','E','ICENS')) -rEPlACe'crf',[ChaR]92))" */
                /* Then we need to connect the new output from PowershellInstance and the origin Powershell script */
                /* == "&('Ne'+'w-'+'obje'+'ct') system.net.webclient;" -> "new-object system.net.webclient" */

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
