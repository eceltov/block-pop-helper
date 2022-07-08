using System;

namespace BlockPopHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var blocks = Parser.ParseBlocks("blocks.txt");
            BoardState state = new(blocks);
            state.MakeMoves(3);
            ResultCollector.PrintResults(Console.Out);
        }
    }
}
