using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;
using QuickGraph.Graphviz;

namespace TraceEvent2.DIFT
{
    class ControlFlowGraph
    {
        private BidirectionalGraph<string, TaggedEdge<string, string>> diftGraph = new BidirectionalGraph<string, TaggedEdge<string, string>>();

        public int NewSubject(string subjectInfo)
        {
            diftGraph.AddVertex(subjectInfo);
            return 0;
        }

        public int NewObject(string objectInfo)
        {
            diftGraph.AddVertex(objectInfo);
            return 0;
        }

        public int NewConnection(string source, string destination, string tag)
        {
            diftGraph.AddEdge(new TaggedEdge<string, string>(source, destination, tag));
            return 0;
        }

        public string ToGraphvizString()
        {
            var graphviz = new GraphvizAlgorithm<string, TaggedEdge<string, string>>(this.diftGraph);
            graphviz.FormatVertex += (sender, args) => args.VertexFormatter.Label = args.Vertex.ToString();
            graphviz.FormatEdge += (sender, args) => args.EdgeFormatter.Label.Value = args.Edge.Tag;
            return graphviz.Generate();
        }

        public void test()
        {
            this.NewSubject("chrome.exe");
            this.NewObject("password.txt");
            this.NewConnection("chrome.exe", "password.txt", "READ_FILE");

            var fs = new FileStream("graphviz.dot", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var dataOut = new StreamWriter(fs);
            dataOut.WriteLine(this.ToGraphvizString());
            dataOut.Flush();
            dataOut.Close();
        }
    }
}
