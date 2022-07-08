using System;
using System.Diagnostics;
using System.IO;

namespace BlockPopHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Max Moves:");
            int moves = int.Parse(Console.ReadLine());

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var blocks = Parser.ParseBlocks("blocks.txt");
            BoardState state = new(blocks);
            state.MakeMoves(moves - 1);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            using (StreamWriter writer = new StreamWriter("out.txt"))
            {
                writer.WriteLine($"Max moves: {moves}");
                writer.WriteLine($"Finished in {elapsedTime}");
                ResultCollector.PrintResults(writer);
            }
        }
    }
}
