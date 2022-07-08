using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPopHelper
{
    class Result
    {
        public Result Clone()
        {
            return new Result { tokens = tokens, chests = (int[])chests.Clone(), movesString = movesString, moves = moves };
        }

        public void AddMove(int x, int y)
        {
            movesString += $" {x}:{y}";
            moves++;
        }

        public string GetMoves()
        {
            return (movesString.Length > 0 ? movesString.Substring(0, movesString.Length - 1) : movesString);
        }

        public override string ToString()
        {
            string s = $"{tokens}";
            foreach (int count in chests)
            {
                s += $" {count}";
            }
            return s;
        }

        public void Print(TextWriter writer)
        {
            if (chests.Sum() == 0 && tokens == 0)
                return;
                
            if (tokens != 0)
                writer.WriteLine($"Tokens: {tokens}");

            if (chests.Sum() != 0)
            {
                string chestString = "Chests:";
                for (int i = 0; i < chests.Length; i++)
                {
                    if (chests[i] > 0)
                        chestString += $" {i + 1}*:{chests[i]}";
                }
                writer.WriteLine(chestString);
            }

            writer.WriteLine(movesString);

            writer.WriteLine("-----------------------------------");
        }

        public int tokens = 0;
        public int[] chests = new int[Constants.MaxChestStars];
        public int moves = 0;

        public string movesString = "Moves:";
    }
}
