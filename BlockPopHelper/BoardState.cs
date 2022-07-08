using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using BlockPopHelper.Blocks;

namespace BlockPopHelper
{
    class BoardState
    {
        public BoardState(Block[,] blocks)
        {
            this.blocks = blocks;
            groups = new();
            result = new();

            FindGroups(0, Constants.BoardWidth - 1);
        }

        BoardState(Block[,] blocks, List<ColorBlockGroup> groups, Result result)
        {
            this.blocks = blocks;
            this.groups = groups;
            this.result = result;
        }

        public void MakeMoves(int remainingNestings)
        {
            List<Task> tasks = new();

            for (int i = 0; i < groups.Count; i++)
            {
                var chestOptions = groups[i].ChestSpawnOptions(blocks);

                if (chestOptions.Count == 0)
                {
                    var newState = Clone();

                    // pop the group of the new state, not the old one
                    var newGroup = newState.groups[i];

                    // choose arbitrary group block position to pop
                    newState.PopGroup(newGroup, newGroup.Blocks[0].X, newGroup.Blocks[0].Y);

                    ResultCollector.Add(newState.result);

                    if (remainingNestings > 0)
                    {
                        if (remainingNestings % 2 == 0)
                            tasks.Add(Task.Run(() => newState.MakeMoves(remainingNestings - 1)));
                        else
                            newState.MakeMoves(remainingNestings - 1);
                    }

                }
                else
                {
                    foreach (var option in chestOptions)
                    {
                        var newState = Clone();

                        // pop the group of the new state, not the old one
                        var newGroup = newState.groups[i];

                        newState.PopGroup(newGroup, option.x, option.y);

                        ResultCollector.Add(newState.result);

                        if (remainingNestings > 0)
                        {
                            if (remainingNestings % 2 == 0)
                                tasks.Add(Task.Run(() => newState.MakeMoves(remainingNestings - 1)));
                            else
                                newState.MakeMoves(remainingNestings - 1);
                        }
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        void PopGroup(ColorBlockGroup group, int x, int y)
        {
            result.AddMove(x, y);

            int count = group.Blocks.Count;

            // handle chest creation
            if (count >= Constants.BlocksNeededForChest)
            {
                // remove the block from the group so that the chest does not get removed later
                group.Blocks.Remove((ColorBlock)blocks[x, y]);

                blocks[x, y] = new ChestBlock(x, y, group.Blocks[0].Color,
                    Math.Min(Constants.MaxChestStars, count - Constants.BlocksNeededForChest + 1));

                if (y == 0)
                {
                    blocks[x, y].AddToResult(result);
                    BreakBlock(blocks[x, y]);
                }
            }

            List<ColorBlockGroup> newGroups = new();

            // preserve groups that cannot be influenced by popping the group
            // groups adjacent to the popped group can be influenced (merging with new blocks)
            foreach (var oldGroup in groups)
            {
                bool intersecting = false;
                foreach (var block in oldGroup.Blocks)
                {
                    if (block.X >= group.LeftBoundary - 1 && block.X <= group.RightBoundary + 1)
                    {
                        intersecting = true;
                        oldGroup.Ungroup();
                        break;
                    }
                }

                if (!intersecting)
                    newGroups.Add(oldGroup);
            }

            groups = newGroups;

            // destroy blocks
            foreach (var block in group.Blocks)
            {
                BreakBlock(block);

                // handle collectible blocks
                while (block.Y == 0 && blocks[block.X, 0].Collectible)
                {
                    blocks[block.X, 0].AddToResult(result);
                    BreakBlock(blocks[block.X, 0]);
                }
            }

            // find new groups (in a radius of 1 bigger on both sides of the popped groups boundary)
            FindGroups(Math.Max(0, group.LeftBoundary - 1), Math.Min(Constants.BoardWidth - 1, group.RightBoundary + 1));
        }

        /// <summary>
        /// Removes a block from the board and moves all blocks above it down.
        /// Creates an Unknown block on top.
        /// Does not handle collectible blocks.
        /// </summary>
        /// <param name="block">The block to be removed.</param>
        void BreakBlock(Block block)
        {
            for (int y = block.Y; y < Constants.BoardHeight - 1; y++)
            {
                blocks[block.X, y] = blocks[block.X, y + 1];
                blocks[block.X, y].Y = y;
            }

            blocks[block.X, Constants.BoardHeight - 1] = new UnknownBlock(block.X, Constants.BoardHeight - 1);
        }

        /// <returns>Returns a deep copy of the board state</returns>
        BoardState Clone()
        {
            Block[,] newBlocks = new Block[Constants.BoardWidth, Constants.BoardHeight];
            for (int x = 0; x < Constants.BoardWidth; x++)
            {
                for (int y = 0; y < Constants.BoardHeight; y++)
                {
                    newBlocks[x, y] = blocks[x, y].Clone();
                }
            }

            List<ColorBlockGroup> newGroups = new();
            foreach (ColorBlockGroup group in groups)
            {
                newGroups.Add(group.Clone(newBlocks));
            }

            return new BoardState(newBlocks, newGroups, result.Clone());
        }

        /// <summary>
        /// Finds all color block groups that intersect the boundaries and adds them to the
        /// groups field.
        /// </summary>
        /// <param name="leftBoundary">Inclusive left boundary.</param>
        /// <param name="rightBoundary">Inclusive right boundary.</param>
        void FindGroups(int leftBoundary, int rightBoundary)
        {
            for (int x = leftBoundary; x <= rightBoundary; x++)
            {
                for (int y = 0; y < Constants.BoardHeight; y++)
                {
                    // skip if block is already part of a group
                    if (blocks[x, y].Grouped)
                        continue;

                    if (blocks[x, y] is ColorBlock colorBlock)
                    {
                        ColorBlockGroup group = new();
                        RecursiveGrouping(group, colorBlock.Color, x, y);
                        if (group.Blocks.Count >= Constants.MinToPop)
                        {
                            group.SetBoundaries();
                            groups.Add(group);
                        }
                        else
                            group.Ungroup();
                    }
                }
            }
        }

        /// <summary>
        /// If the target block is of the correct color, it will get grouped.
        /// </summary>
        /// <param name="group">The group to add blocks.</param>
        /// <param name="x">The X coordinate of the block to check.</param>
        /// <param name="y">The Y coordinate of the block to check.</param>
        /// <returns></returns>
        void RecursiveGrouping(ColorBlockGroup group, BlockColors color, int x, int y)
        {
            Block block = blocks[x, y];

            if (block.Grouped)
                return;

            ColorBlock colorBlock = block as ColorBlock;
            if (colorBlock != null && colorBlock.Color == color)
            {
                colorBlock.Grouped = true;
                group.Blocks.Add(colorBlock);

                if (x > 0)
                    RecursiveGrouping(group, color, x - 1, y);
                if (x < Constants.BoardWidth - 1)
                    RecursiveGrouping(group, color, x + 1, y);
                if (y > 0)
                    RecursiveGrouping(group, color, x, y - 1);
                if (y < Constants.BoardHeight - 1)
                    RecursiveGrouping(group, color, x, y + 1);
            }

        } 


        Block[,] blocks;
        List<ColorBlockGroup> groups;
        Result result;
    }
}
