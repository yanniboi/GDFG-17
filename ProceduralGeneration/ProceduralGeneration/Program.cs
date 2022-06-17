using Misc;
using Noise;
using AreaGeneration;
using System;
using System.Drawing;

namespace ProcCGen
{
    class Program
    {
        public static void Main(string[] args)
        {
            IScene scene = null;
            scene = new Scene_Zoom();
            scene = new Scene_Scroll();
            scene = new Scene_Static();
            scene = new Scene_Generation();
            scene.Start();
        }
    }

    public interface IScene
    {
        void Start();
    }

    class Scene_Generation : IScene
    {
        public void Start()
        {
            Random random = new Random();
            int seed = 1337;

            while (true)
            {
                AreaGenerator2D generator = new AreaGenerator2D(42, 5, 3);

                int width = 5, height = 5, tilesX = 50, tilesY = 30;

                AreaGenerator2D.AreaConfig config = new AreaGenerator2D.AreaConfig(5, 5, 50, 30, seed)
                    .Set(
                    new AreaGenerator2D.TileRule(-10, .2f, .5f, 0),
                    new AreaGenerator2D.TileRule(.1f, 10f, .5f, 1))
                    .Set(
                    new AreaGenerator2D.AreaSpotRemover(0, 2, 1, 0),
                    new AreaGenerator2D.AreaBorderRule(0, 5, 8, 1), // Create Border
                                                                    // new AreaGenerator2D.AreaBorderRule(20, 5, 8, 1), // Create Ring
                    new AreaGenerator2D.AreaFillRule(0, 40, 0, 1),  // Convert small holes into walls
                    new AreaGenerator2D.AreaFillRule(0, 40, 1, 0), // Convert small walls into emptiness
                    new AreaGenerator2D.AreaConnector(1, 0) // Make Sure... everything is connected
                    );

                int[,] states = generator.Generate(config);

                int viewX = 0,
                    viewY = 0;

                while (true)
                {
                    string[] lines = new string[tilesY];

                    for (int y = 0; y < tilesY; y++)
                    {
                        string line = "";
                        for (int x = 0; x < tilesX; x++)
                        {
                            int fX = viewX * tilesX + x;
                            int fY = viewY * tilesY + y;

                            int state = states[fX, fY];
                            if (state == 0)
                            {
                                //line += (x % 5 == 0 || y % 5 == 0 ? "," : ".");
                                line += ".";
                            }
                            else if (state == 1)
                            {
                                //line += (x % 5 == 0 || y % 5 == 0 ? "x" : "X");
                                line += "X";
                            }
                        }

                        lines[y] = line;
                    }

                    Console.SetCursorPosition(0, 0);
                    for (int i = tilesY - 1; i >= 0; i--)
                    // for (int i = 0; i < tilesY; i++)
                    {
                        Console.WriteLine(lines[i]);
                    }
                    Console.WriteLine($"{viewX}/{viewY}");

                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        viewY = (viewY + 1).Loop(height);
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        viewY = (viewY - 1).Loop(height);
                    }
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        viewX = (viewX + 1).Loop(width);
                    }
                    else if (key.Key == ConsoleKey.LeftArrow)
                    {
                        viewX = (viewX - 1).Loop(width);
                    }
                    else if (key.Key == ConsoleKey.S)
                    {
                        Bitmap bmp = new Bitmap(width * tilesX, height * tilesY);
                        for (int y = height * tilesY - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < width * tilesX; x++)
                            {
                                int fY = height * tilesY - 1 - y;
                                int state = states[x, y];

                                if (state == 0)
                                {
                                    bmp.SetPixel(x, fY, fY % tilesY == 0 || x % tilesX == 0 ? Color.Gray : Color.Black);
                                }
                                else if (state == 1)
                                {
                                    bmp.SetPixel(x, fY, fY % tilesY == 0 || x % tilesX == 0 ? Color.Silver : Color.White);
                                }
                            }
                        }

                        bmp.Save("Debug.png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (key.Key == ConsoleKey.R)
                    {
                        break;
                    }
                }

                seed = random.Next();
            }
        }
    }

    class Scene_Zoom : IScene
    {
        public void Start()
        {
            Random random = new Random();

            INoise generator = new GradientNoise(1);

            int height = 30;
            int width = 50;
            string[] lines = new string[height];
            float zoom = 0;

            string heightMap = ".,:;+#X";

            float seedOffsetX = 347,
                 seedOffsetY = 124;

            bool reseed = true;

            float amp = 0, fre = 0, zoomBase = 0;
            int layers = 0;

            while (true)
            {
                if (reseed)
                {
                    generator = new GradientNoise(random.Next());

                    amp = (float)random.NextDouble() * 3f;
                    fre = (float)random.NextDouble() * 3f;
                    zoomBase = 1 + ((float)random.NextDouble() * 20f);
                    layers = 2 + random.Next(6);

                    reseed = false;
                }

                System.Threading.Thread.Sleep(10);

                float finalZoom = zoomBase + (zoom).PingPong(6);

                for (int y = 0; y < height; y++)
                {
                    string line = "";
                    for (int x = 0; x < width; x++)
                    {
                        float result = generator.Get(new NoiseConfig(layers, amp, fre),
                            seedOffsetX + (x / finalZoom),
                            seedOffsetY + (y / finalZoom), 1337);


                        int idx = (int)(result * 100 * heightMap.Length / 100);
                        idx = Math.Max(0, Math.Min(heightMap.Length - 1, idx));

                        line += heightMap[idx];
                    }

                    lines[y] = line;
                }

                zoom += .05f;

                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < height; i++)
                {
                    Console.WriteLine(lines[i]);
                }

                while (Console.KeyAvailable)
                {
                    Console.ReadKey();
                    reseed = true;
                }
            }
        }
    }

    class Scene_Scroll : IScene
    {
        public void Start()
        {
            Random random = new Random();

            INoise generator = new GradientNoise(1);

            int length = 30;
            string[] lines = new string[length];
            float value = 0;
            int stringLength = 50;

            string heightMap = ".,:;+#X";

            float amp = 1, fre = 1, zoom = 1;
            int layers = 1;

            while (true)
            {
                System.Threading.Thread.Sleep(10);

                for (int i = 0; i < length; i++)
                {
                    float result = generator.Get(new NoiseConfig(layers, amp, fre), (i * .3f) / zoom, value / zoom);
                    //result = result.PingPong(1);

                    int idx = (int)(result * 100 * heightMap.Length / 100);
                    idx = Math.Max(0, Math.Min(heightMap.Length - 1, idx));
                    int lineIdx = length - i - 1;
                    lines[lineIdx] += heightMap[idx];
                    if (lines[lineIdx].Length > stringLength)
                        lines[lineIdx] = lines[lineIdx].Substring(lines[lineIdx].Length - stringLength);
                }

                value += .01f;

                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine(lines[i]);
                }

                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Enter)
                    {
                        generator = new GradientNoise(random.Next());
                    }
                    else if (key == ConsoleKey.Spacebar)
                    {
                        amp = 1 + ((float)random.NextDouble() * 5);
                        fre = 1 + ((float)random.NextDouble() * 5);
                        zoom = 1 + ((float)random.NextDouble() * 15);
                        layers = 1 + random.Next(8);
                    }

                }
            }
        }
    }

    class Scene_Static : IScene
    {
        public void Start()
        {
            Random random = new Random();


            int height = 30;
            int width = 80;
            string[] lines = new string[height];

            string heightMap = ".,:;+#X";

            float seedOffsetX = 347,
                 seedOffsetY = 124;

            float amp = 10, fre = 1, zoom = 35;
            int layers = 6;
            // 5.784183, 2.14742923, 33.03007, 5
            // 5.54354954, 1.40334213, 29.0192375, 5
            // 10, 1, 35, 6
            while (true)
            {
                INoise generator = new GradientNoise(random.Next());

                System.Threading.Thread.Sleep(10);

                for (int y = 0; y < height; y++)
                {
                    string line = "";
                    for (int x = 0; x < width; x++)
                    {
                        float result = generator.Get(new NoiseConfig(layers, amp, fre),
                            seedOffsetX + x / zoom,
                            seedOffsetY + y / zoom);

                        int idx = (int)(result * 100 * heightMap.Length / 100);
                        idx = Math.Max(0, Math.Min(heightMap.Length - 1, idx));

                        line += heightMap[idx];
                    }

                    lines[y] = line;
                }

                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < height; i++)
                {
                    Console.WriteLine(lines[i]);
                }

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Enter)
                {
                    generator = new GradientNoise(random.Next());
                }
                else if (key == ConsoleKey.Spacebar)
                {
                    amp = 1 + ((float)random.NextDouble() * 5);
                    fre = 1 + ((float)random.NextDouble() * 5);
                    zoom = 20 + ((float)random.NextDouble() * 20);
                    layers = 1 + random.Next(8);
                }
            }
        }
    }
}