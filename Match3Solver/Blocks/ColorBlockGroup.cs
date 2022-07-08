using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver.Blocks
{
    class ColorBlockGroup
    {
        public ColorBlockGroup()
        {
            Blocks = new();
        }

        public override string ToString()
        {
            string s = "";
            foreach (var block in Blocks)
            {
                s += block.ToString() + " ";
            }
            return s.Substring(0, s.Length - 1);
        }

        public void Ungroup()
        {
            foreach (ColorBlock block in Blocks)
            {
                block.Grouped = false;
            }
        }

        public List<(int x, int y)> ChestSpawnOptions(Block[,] blocks)
        {
            List<(int x, int y)> options = new();

            if (Blocks.Count < Constants.BlocksNeededForChest)
                return options;

            // if a chest is spawnable on the bottom, there is no need to find other bottom
            // spawning positions
            bool foundBottomChest = false;

            BlockColors color = Blocks[0].Color;
            foreach (ColorBlock block in Blocks)
            {
                // if the block does not have a block of the same color beneath it, add it
                if (block.Y == 0 || blocks[block.X, block.Y - 1].Color != color)
                {
                    if (block.Y == 0 && foundBottomChest)
                        continue;

                    if (block.Y == 0)
                        foundBottomChest = true;

                    options.Add((block.X, block.Y));
                }
            }

            return options;
        }

        public void SetBoundaries()
        {
            int min = Blocks[0].X;
            int max = Blocks[0].X;

            foreach (ColorBlock block in Blocks)
            {
                if (block.X < min)
                {
                    min = block.X;
                }

                if (block.X > max)
                {
                    max = block.X;
                }
            }

            LeftBoundary = min;
            RightBoundary = max;
        }

        /// <summary>
        /// Clones the color block group so that the block reference the same blocks as in the
        /// provided board state (it is expected that the board state is cloned first,
        /// then the group).
        /// </summary>
        /// <param name="boardState"></param>
        /// <returns></returns>
        public ColorBlockGroup Clone(Block[,] blocks)
        {
            List<ColorBlock> newBlocks = new();
            foreach (ColorBlock colorBlock in Blocks)
            {
                newBlocks.Add((ColorBlock)blocks[colorBlock.X, colorBlock.Y]);
            }

            return new ColorBlockGroup { Blocks = newBlocks, LeftBoundary = LeftBoundary, RightBoundary = RightBoundary };
        }

        public int GetColumnTopBoundary(int x)
        {
            int max = -1;

            foreach (ColorBlock block in Blocks)
            {
                if (block.X == x && block.Y > max)
                {
                    max = block.Y;
                }
            }

            if (max == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Error: Column is out of range");
            }

            return max;
        }

        public int GetColumnBottomBoundary(int x)
        {
            int min = -1;

            foreach (ColorBlock block in Blocks)
            {
                if (block.X == x && (block.Y < min || min == -1))
                {
                    min = block.Y;
                }
            }

            if (min == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Error: Column is out of range");
            }

            return min;
        }

        public List<ColorBlock> Blocks { get; init; }

        int _leftBoundary = -1;
        public int LeftBoundary
        {
            get
            {
                if (_leftBoundary < 0)
                {
                    throw new FieldAccessException("Error: Accessing uninitialized boundary.");
                }

                return _leftBoundary;
            }
            private set
            {
                _leftBoundary = value;
            }
        }

        int _rightBoundary = -1;
        public int RightBoundary
        {
            get
            {
                if (_rightBoundary < 0)
                {
                    throw new FieldAccessException("Error: Accessing uninitialized boundary.");
                }

                return _rightBoundary;
            }
            private set
            {
                _rightBoundary = value;
            }
        }
    }
}
