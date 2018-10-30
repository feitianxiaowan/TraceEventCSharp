using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TraceEvent2
{
    class Ngram
    {
        private int N;
        public int n
        {
            get { return N; }
            set { N = value; }
        }

        private Dictionary<string, int> ngramCounter = new Dictionary<string, int>();

        private string[] nameBuffer;
        private int nameBufferOffset = 0;
        private string nameJoint()
        {
            StringBuilder ngram = new StringBuilder();
            for(int i = 0; i < N; i++)
            {
                ngram.Append(nameBuffer[(i + nameBufferOffset) % N]).Append(",");
            }
            return ngram.ToString();
        }


        public Ngram()
        {
            N = 2;
            nameBuffer = new string[N];
        }

        public Ngram(int N)
        {
            this.N = N;
            nameBuffer = new string[N];
        }

        public void Counter(string name)
        {
            nameBuffer[nameBufferOffset] = name;
            string ngram = nameJoint();
            nameBufferOffset = (nameBufferOffset + 1) % N;

            if (ngramCounter.ContainsKey(ngram))
            {
                ngramCounter[ngram] += 1;
            }
            else
            {
                ngramCounter[ngram] = 1;
            }
        }

        public void PrintCounterResult(TextWriter dataOut)
        {
            var ngramSorted = from ngram in ngramCounter orderby ngram.Value descending select ngram;

            foreach(var kvp in ngramSorted)
            {
                dataOut.WriteLine(kvp.Key + ":" + kvp.Value) ;
            }
        }
    }
}
