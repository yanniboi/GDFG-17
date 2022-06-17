namespace Misc
{
    public struct Float2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Float2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"{{{this.X}, {this.Y}}}";
        }
    }
}