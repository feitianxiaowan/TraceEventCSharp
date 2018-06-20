using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Diagnostics.Tracing;

namespace TraceEvent2
{ 
    class CoOccurenceMatrix
    {
        public static Dictionary<string, Dictionary<string, int>> coOccurenceMatrix = new Dictionary<string, Dictionary<string, int>>();

        private static List<Dictionary<string, int>> stepCounterList = new List<Dictionary<string, int>>();
        private static int currentStep = -1;
        private static Dictionary<string, int> windowsCount = new Dictionary<string, int>();

        private static readonly int stepSize = 1000;
        private static readonly int windowSize = stepSize * 1;

        private static int eventCount = -1;

        public static System.IO.TextWriter logOut = Console.Out;
        public static TextWriter dataOut = new StreamWriter(new FileStream("CoOccurenceMatrix.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite));

        public static void ProcessCoOccurence(TraceEvent data)
        {
            eventCount++;
            if(eventCount % stepSize == 0)
            {
                if(eventCount % windowSize == 0 && eventCount != 0)
                {
                    StatisticEvent();
                    stepCounterList.Remove(stepCounterList[0]);
                    currentStep--;
                }
                stepCounterList.Add(new Dictionary<string, int>());
                currentStep++;
            }

            if (!stepCounterList[currentStep].ContainsKey(data.EventName))
            {
                stepCounterList[currentStep].Add(data.EventName, 1);
            }
            else
            {
                stepCounterList[currentStep][data.EventName] += 1;
            }
        }

        private static void StatisticEvent()
        {
            foreach (var iter in stepCounterList)
            {
                foreach (var iter2 in iter)
                {
                    if (!windowsCount.ContainsKey(iter2.Key))
                    {
                        windowsCount.Add(iter2.Key, iter2.Value);
                    }
                    else
                    {
                        windowsCount[iter2.Key] += iter2.Value;
                    }
                }
            }

            foreach(var iter in windowsCount)
            {
                if (!coOccurenceMatrix.ContainsKey(iter.Key))
                {
                    coOccurenceMatrix.Add(iter.Key, new Dictionary<string, int>());
                }
                foreach(var iter2 in windowsCount)
                {
                    if (!coOccurenceMatrix[iter.Key].ContainsKey(iter2.Key))
                    {
                        coOccurenceMatrix[iter.Key].Add(iter2.Key, iter2.Value);
                    }
                    else
                    {
                        coOccurenceMatrix[iter.Key][iter2.Key] += iter2.Value;
                    }
                }
            }
            windowsCount.Clear();
        }

        public static void PrintCoOccurenceMatrix()
        {
            logOut.WriteLine(eventCount);
            for (var index = 0; index <coOccurenceMatrix.Count(); index++)
            {
                dataOut.Write(coOccurenceMatrix.ElementAt(index).Key);
                dataOut.Write(',');
            }
            dataOut.WriteLine();

            for (var index = 0; index < coOccurenceMatrix.Count(); index++)
            {
                dataOut.Write(coOccurenceMatrix.ElementAt(index).Key);
                dataOut.Write(',');
                for (var index2 = 0; index2 < coOccurenceMatrix.Count(); index2++)
                {
                    if (coOccurenceMatrix.ElementAt(index).Value.ContainsKey(coOccurenceMatrix.ElementAt(index2).Key))
                    {
                        dataOut.Write(coOccurenceMatrix.ElementAt(index).Value[coOccurenceMatrix.ElementAt(index2).Key]);
                        dataOut.Write(',');
                    }
                    else
                    {
                        dataOut.Write(0);
                        dataOut.Write(',');
                    }
                }
                dataOut.WriteLine();
            }
        }

    }
}
