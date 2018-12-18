using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day15
    {
        enum CellState
        {
            Clear,
            Wall,
            Elf,
            Golbin
        }

        static CellState CharToCellState(char value)
        {
            switch (value)
            {
                case '.':
                    return CellState.Clear;

                case '#':
                    return CellState.Wall;

                case 'E':
                    return CellState.Elf;

                case 'G':
                    return CellState.Golbin;

                default:
                    Oh.Bugger();
                    return CellState.Clear;
            }
        }

        static char CellStateToChar(CellState cellState)
        {
            switch (cellState)
            {
                case CellState.Clear:
                    return '.';

                case CellState.Wall:
                    return '#';

                case CellState.Elf:
                    return 'E';

                case CellState.Golbin:
                    return 'G';

                default:
                    Oh.Bollocks();
                    return 'X';
            }
        }

        [Fact]
        void TestNavigation()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            environment.DebugDump(CellStateToChar);
        }
    }
}
