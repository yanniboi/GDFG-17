namespace Misc
{
    public struct Int2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Int2(int x, int y)
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
