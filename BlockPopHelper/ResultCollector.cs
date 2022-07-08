using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPopHelper
{
    static class ResultCollector
    {
        static Dictionary<string, Result> bestResults = new();

        public static void Add(Result result)
        {
            if (result.tokens == 0 && result.chests.Sum() == 0)
                return;

            string resString = result.ToString();

            if (!bestResults.ContainsKey(resString) || (bestResults.ContainsKey(resString) && bestResults[resString].moves > result.moves))
            {
                // update best result if a better one is found
                lock (bestResults)
                {
                    if (bestResults.ContainsKey(resString) && bestResults[resString].moves > result.moves)
                    {
                        bestResults[resString] = result;
                    }
                    else if (!bestResults.ContainsKey(resString))
                    {
                        bestResults.Add(resString, result);
                    }
                }
            }
        }

        public static void PrintResults(TextWriter writer)
        {
            writer.WriteLine("------------- Results -------------");

            foreach (var (_, result) in bestResults)
            {
                result.Print(writer);
            }
        }
    }
}
