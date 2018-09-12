using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TraceEvent2
{
    class MalPowershellScriptDetector
    {
        private List<string> signatureList = new List<string>();
        private List<Regex> signatureRegList = new List<Regex>();
        private string signaturePath = @"C:\Users\xiaowan\Documents\Git\TraceEventCSharp\TraceEvent2\MalPowershellScript.sig";

        public static TextWriter logOut = Console.Out;

        public MalPowershellScriptDetector()
        {
            ReadInSignature();
        }

        public MalPowershellScriptDetector(string sigPath)
        {
            signaturePath = sigPath;
            ReadInSignature();
        }

        protected void ReadInSignature()
        {
            FileStream fs;
            TextReader dataIn;
            string signature;
            Regex re;

            try
            {
                fs = new FileStream(signaturePath, FileMode.Open, FileAccess.ReadWrite);
                dataIn = new StreamReader(fs);
            }
            catch
            {
                logOut.WriteLine("Can't open signature file:" + signaturePath);
                return;
            }

            try
            {
                while ((signature = dataIn.ReadLine()) != null)
                {
                    signatureList.Add(signature.Trim());
                    try
                    {
                        re = new Regex(signature.Trim(), RegexOptions.Compiled);
                    }
                    catch
                    {
                        logOut.WriteLine("Regex expression error: " + signature);
                        continue;
                    }
                    signatureRegList.Add(re);
                }
            }
            catch
            {

            }
        }

        public string Match(string sample)
        {
            foreach(var signature in signatureRegList)
            {
                if (signature.IsMatch(sample))
                {
                    return signature.ToString();
                }
            }

            return null;
        }
    }
}
