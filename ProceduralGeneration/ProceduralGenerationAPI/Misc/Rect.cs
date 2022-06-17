namespace Misc
{
    public class Rect
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }


        public Rect(int offsetX, int offsetY, int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.OffsetX = offsetX;
            this.OffsetY = offsetY;

            if (this.Width < 0)
            {
                this.Width *= -1;
                this.OffsetX -= (this.Width - 1);
            }

            if (this.Height < 0)
            {
                this.Height *= -1;
                this.OffsetY -= (this.Height - 1);
            }
        }

    }
}
