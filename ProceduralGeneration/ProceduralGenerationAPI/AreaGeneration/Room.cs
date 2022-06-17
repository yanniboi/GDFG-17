using Misc;
using System.Collections.Generic;

namespace AreaGeneration
{
    public class Area
    {
        public List<Int2> Tiles { get; private set; }
        public List<Int2> Borders { get; private set; }


        public Area(List<Int2> tiles)
        {
            this.Tiles = tiles;
            this.Borders = new List<Int2>();
        }

    }
}