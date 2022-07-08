using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver.Blocks
{
    abstract class Block
    {
        protected Block()
        {
            Grouped = false;
        }

        public virtual void AddToResult(Result result) { }

        public Block Clone()
        {
            return (Block)MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }

        public int X { get; set; }
        public int Y { get; set; }

        public BlockColors Color { get; init; }
        
        // whether the block gets collected when it reaches the bottom of the board
        public bool Collectible { get; init; }

        // whether the block is assigned to a group.
        public bool Grouped { get; set; }

        // whether the block can be popped if the conditions are right
        protected bool Poppable { get; init; }
    }
}
