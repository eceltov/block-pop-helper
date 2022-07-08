using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlockPopHelper.Blocks
{   
    class ColorBlock : Block
    {
        public ColorBlock(int x, int y, BlockColors color)
        {
            if (color == BlockColors.None)
            {
                throw new NotSupportedException("Error:  Color Blocks need a color.");
            }

            Color = color;
            Collectible = false;
            Poppable = true;
            X = x;
            Y = y;
        }
    }
}
