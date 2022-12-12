using System;
using System.Collections.Generic;
using Utils.Alogrithms;

namespace Utils
{
    public abstract class GridAdapterBase<T> : Astar.IGraphAdapter<(int x, int y)>
    {
        public readonly T[,] Grid;

        public GridAdapterBase(T[,] grid)
        {
            Grid = grid;
        }

        public abstract IEnumerable<(int x, int y)> GetLinked((int x, int y) node);

        public int GetMoveCost((int x, int y) from, (int x, int y) to)
        {
            return 1;
        }

        public int GetScore((int x, int y) from, (int x, int y) to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }
    }
}
