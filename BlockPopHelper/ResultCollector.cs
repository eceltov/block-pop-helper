using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPopHelper
{
    static class ResultCollector
    {
        static List<Result> results = new();

        public static void Add(Result result)
        {
            if (result.tokens != 0 || result.chests.Sum() > 0)
                results.Add(result);
        }

        public static void PrintResults(TextWriter writer)
        {
            Dictionary<string, Result> bestResults = new();

            foreach (var result in results)
            {
                string resString = result.ToString();
                // update best result if a better one is found
                if (bestResults.ContainsKey(resString) && bestResults[resString].moves > result.moves)
                {
                    bestResults[resString] = result;
                }
                else if (!bestResults.ContainsKey(resString))
                {
                    bestResults.Add(resString, result);
                }
            }

            foreach (var (_, result) in bestResults)
            {
                result.Print(writer);
            }
        }
    }
}
