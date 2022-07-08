using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Match3Solver.Blocks;

namespace Match3Solver
{
    static class Parser
    {
        public static Block[,] ParseBlocks(string fileName)
        {
            Block[,] blocks = new Block[Constants.BoardWidth, Constants.BoardHeight];
            using (StreamReader reader = new StreamReader(fileName))
            {

                string line;
                int y = Constants.BoardHeight - 1;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(' ');

                    if (tokens.Length != Constants.BoardWidth)
                        throw new FormatException("Error: Bad file format.");

                    for (int x = 0; x < Constants.BoardWidth; x++)
                    {
                        switch (tokens[x])
                        {
                            case "t":
                                blocks[x, y] = new TokenBlock(x, y);
                                break;
                            case "r":
                                blocks[x, y] = new ColorBlock(x, y, BlockColors.Red);
                                break;
                            case "y":
                                blocks[x, y] = new ColorBlock(x, y, BlockColors.Yellow);
                                break;
                            case "g":
                                blocks[x, y] = new ColorBlock(x, y, BlockColors.Green);
                                break;
                            case "b":
                                blocks[x, y] = new ColorBlock(x, y, BlockColors.Blue);
                                break;
                            case "o":
                                blocks[x, y] = new ColorBlock(x, y, BlockColors.Orange);
                                break;
                            default:
                                // parse chests
                                if (tokens[x].Length != 3 || tokens[x][0] != 'c')
                                    throw new FormatException("Error: Bad file format (unknown symbol).");

                                int stars = int.Parse(tokens[x][1].ToString());
                                if (stars < 1 || stars > Constants.MaxChestStars)
                                    throw new FormatException("Error: Bad file format (too many stars).");

                                switch (tokens[x][2])
                                {
                                    case 'r':
                                        blocks[x, y] = new ChestBlock(x, y, BlockColors.Red, stars);
                                        break;
                                    case 'y':
                                        blocks[x, y] = new ChestBlock(x, y, BlockColors.Yellow, stars);
                                        break;
                                    case 'g':
                                        blocks[x, y] = new ChestBlock(x, y, BlockColors.Green, stars);
                                        break;
                                    case 'b':
                                        blocks[x, y] = new ChestBlock(x, y, BlockColors.Blue, stars);
                                        break;
                                    case 'o':
                                        blocks[x, y] = new ChestBlock(x, y, BlockColors.Orange, stars);
                                        break;
                                    default:
                                        throw new FormatException("Error: Bad file format (unknown symbol).");
                                }
                                break;
                        }
                    }

                    y--;
                }

                if (y != -1)
                    throw new FormatException("Error: Bad file format.");
            }

            return blocks;
        }
    }
}
