using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver.Blocks
{
    class TokenBlock : Block
    {
        public TokenBlock(int x, int y)
        {
            Color = BlockColors.None;
            Collectible = true;
            Poppable = false;
            X = x;
            Y = y;
        }

        public override void AddToResult(Result result)
        {
            result.tokens += 1;
        }
    }
}
