using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver.Blocks
{
    class ChestBlock : Block
    {
        public ChestBlock(int x, int y, BlockColors color, int stars)
        {
            if (color == BlockColors.None)
            {
                throw new NotSupportedException("Error: Chest Blocks need a color.");
            }

            if (stars > Constants.MaxChestStars)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(stars), 
                    "Error: Stars must be lower than " + Constants.MaxChestStars.ToString()
                );
            }

            Color = color;
            Collectible = true;
            Poppable = false;
            X = x;
            Y = y;
            Stars = stars;
        }

        public int Stars { get; init; }

        public override void AddToResult(Result result)
        {
            result.chests[Stars] += 1;
        }
    }
}
