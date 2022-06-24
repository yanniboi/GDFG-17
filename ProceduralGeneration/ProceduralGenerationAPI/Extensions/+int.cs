namespace System
{
    public static partial class Extension
    {
        public static int Pow(this int value, int power)
        {
            // Should be sufficent
            return (int)Math.Pow(value, power);

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

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                value = min;

            if (value > max)
                value = max;

            return value;
        }

        public static int ClampMin(this int value, int min)
        {
            return value.Max(value, min);
        }

        public static int ClampMax(this int value, int max)
        {
            return value.Min(value, max);
        }

        public static int Min(this int value, params int[] other)
        {
            for (int i = 0; i < other.Length; i++)
            {
                value = Math.Min(value, other[i]);
            }

            return value;
        }

        public static int Max(this int value, params int[] other)
        {
            for (int i = 0; i < other.Length; i++)
            {
                value = Math.Max(value, other[i]);
            }

            return value;
        }

        public static int Loop(this int value, int length)
        {
            int temp = (value - (int)Math.Floor((decimal)value / length) * length);
            return temp.Clamp(0, length);
        }

        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }

    }
}