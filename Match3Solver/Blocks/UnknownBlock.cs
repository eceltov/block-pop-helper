using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver.Blocks
{
    class UnknownBlock : Block
    {
        public UnknownBlock(int x, int y)
        {
            Color = BlockColors.None;
            Collectible = false;
            Poppable = false;
            X = x;
            Y = y;
        }
    }
}
