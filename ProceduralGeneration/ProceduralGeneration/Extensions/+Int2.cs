using System;

namespace Misc
{
    public static partial class Extension
    {
        public static int ManhattanDistance(this Int2 a, Int2 b)
        {
            int deltaX = a.X - b.X;
            int deltaY = a.Y - b.Y;

            return deltaX.Abs() + deltaY.Abs();

            //if (power == 0)
            //    return 1;

            //if (power == 1)
            //    return value;

            //int toReturn = value;
            //for (int i = 1; i < power; i++)
            //{
            //    toReturn *= value;
            //}

            //return toReturn;
        }
    }
}