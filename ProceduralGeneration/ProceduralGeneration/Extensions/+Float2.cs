using System;

namespace Misc
{
    public static partial class Extension
    {
        public static float Dot(this Float2 a, Float2 b)
        {
            float toReturn = 0;

            toReturn += a.X * b.X;
            toReturn += a.Y * b.Y;

            return toReturn;
        }
    }
}