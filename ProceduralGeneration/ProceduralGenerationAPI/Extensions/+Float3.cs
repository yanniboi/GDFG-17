namespace Misc
{
    public static partial class Extension
    {
        public static float Dot(this Float3 a, Float3 b)
        {
            float toReturn = 0;

            toReturn += a.X * b.X;
            toReturn += a.Y * b.Y;
            toReturn += a.Z * b.Z;

            return toReturn;
        }
    }
}