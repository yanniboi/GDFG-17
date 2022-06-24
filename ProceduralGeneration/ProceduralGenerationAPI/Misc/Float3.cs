namespace Misc
{
    public struct Float3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float this[int idx]
        {
            set
            {
                if (idx == 0)
                    this.X = value;
                else if (idx == 1)
                    this.Y = value;
                else if (idx == 2)
                    this.Z = value;

            }
        }

        public Float3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return $"{{{this.X}, {this.Y}, {this.Z}}}";
        }
    }
}