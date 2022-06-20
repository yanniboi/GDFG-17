using Misc;
using Noise;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AreaGeneration
{
    public class AreaGenerator2D
    {
        public Action<int> OnGenerationStart { get; set; }
        public Action<int> OnGenerationProgress { get; set; }

        GradientNoise Noise { get; set; }
        Random Random { get; set; }

        public int Seed { get; private set; }

        int Progress { get; set; }
        int ProgressMax { get; set; }

        public AreaGenerator2D(int seed)
        {
            this.Seed = seed;
        }

        public int[,] Generate(AreaConfig config)
        {
            this.ProgressMax = config.AreaRules.Length + 2; // Area Rules + ApplyTileRules + GetNoise
            this.Progress = 0;

            OnGenerationStart?.Invoke(this.ProgressMax);

            this.Noise = new GradientNoise(this.Seed);
            this.Random = new Random(this.Seed * 1337);

            int tileCountX = config.AreaTilesX * config.Width;
            int tileCountY = config.AreaTilesY * config.Height;

            float scale = .1f;

            float noiseStartX = 0,
                noiseStartY = 0;

            float[,] values = new float[tileCountX, tileCountY];

            // Get Noise
            this.GetNoiseValues(tileCountX, tileCountY, scale, noiseStartX, noiseStartY, values);

            this.Progress++;
            this.UpdateProgress();


            int[,] states = new int[tileCountX, tileCountY];

            // Apply Tile Rules
            this.ApplyTileRules(config, tileCountX, tileCountY, values, states);

            this.Progress++;
            this.UpdateProgress();

            // Apply Area Rules
            this.ApplyAreaRules(config, values, states);

            return states;
        }

        private void ApplyAreaRules(AreaConfig config, float[,] values, int[,] states)
        {
            foreach (var areaRule in config.AreaRules)
            {
                areaRule.Apply(this.Random, values, states);

                this.Progress++;
                this.UpdateProgress();
            }
        }

        private void ApplyTileRules(AreaConfig config, int tileCountX, int tileCountY, float[,] values, int[,] states)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                for (int x = 0; x < tileCountX; x++)
                {
                    float value = values[x, y];
                    TileRule[] validRules = config.TileRules.Where(rule => rule.ValueMin <= value && rule.ValueMax >= value).ToArray();

                    if (validRules.Length > 0)
                    {
                        double sum = validRules.Sum(rule => rule.Chance);
                        double rng = this.Random.NextDouble() * sum;
                        int ruleIndex = 0; // this.Random.Next(validRules.Length);

                        for (int i = 0; i < validRules.Length; i++)
                        {
                            if (validRules[i].Chance <= rng)
                            {
                                ruleIndex = i;
                                break;
                            }

                            rng -= validRules[i].Chance;
                        }

                        states[x, y] = validRules[ruleIndex].State;
                    }
                }
            }
        }

        private void GetNoiseValues(int tileCountX, int tileCountY, float scale, float noiseStartX, float noiseStartY, float[,] values)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                for (int x = 0; x < tileCountX; x++)
                {
                    float fX = noiseStartX + (x * scale);
                    float fY = noiseStartY + (y * scale);

                    values[x, y] = this.Noise.Get(new NoiseConfig(5, 12, 1), fX, fY);
                }
            }
        }

        private void UpdateProgress()
        {
            OnGenerationProgress?.Invoke(this.Progress);
        }


        public class AreaConfig
        {
            public int Width { get; private set; }
            public int Height { get; private set; }

            public int AreaTilesX { get; private set; }
            public int AreaTilesY { get; private set; }

            public TileRule[] TileRules { get; private set; }
            public AreaRule[] AreaRules { get; private set; }


            public AreaConfig(int width, int height, int areaTilesX, int areaTilesY)
            {
                this.Width = width;
                this.Height = height;

                this.AreaTilesX = areaTilesX;
                this.AreaTilesY = areaTilesY;
            }

            public AreaConfig Set(params TileRule[] tileRules)
            {
                this.TileRules = tileRules;

                return this;
            }

            public AreaConfig Set(params AreaRule[] areaRules)
            {
                this.AreaRules = areaRules;

                return this;
            }
        }

        public class TileRule
        {
            public float ValueMin { get; private set; }
            public float ValueMax { get; private set; }

            // In Case multiple Rules can be applied, throw a dice
            public float Chance { get; private set; }
            // The resulting State/ID/Tile
            public int State { get; private set; }

            public TileRule(float valueMin, float valueMax, float chance, int state)
            {
                this.ValueMin = valueMin;
                this.ValueMax = valueMax;

                this.Chance = chance;
                this.State = state;
            }
        }

        public abstract class AreaRule
        {
            public abstract void Apply(Random random, float[,] values, int[,] states);

            protected bool IsValidTile(int[,] states, int x, int y)
            {
                if (x < 0 || x >= states.GetLength(0) || y < 0 || y >= states.GetLength(1))
                    return false;

                return true;
            }
        }

        public class AreaSpotRemover : AreaRule
        {
            public int CountMin { get; private set; }
            public int CountMax { get; private set; }
            public int Range { get; private set; }
            public int State { get; private set; }

            public AreaSpotRemover(int countMin, int countMax, int range, int state)
            {
                this.CountMin = countMin;
                this.CountMax = countMax;
                this.Range = range.Abs();
                this.State = state;
            }


            public override void Apply(Random random, float[,] values, int[,] states)
            {
                int width = values.GetLength(0);
                int height = values.GetLength(1);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (states[x, y] == this.State)
                            continue;

                        int count = 0;
                        int stateCheck = states[x, y];
                        for (int oX = -this.Range; oX <= this.Range; oX++)
                        {
                            for (int oY = -this.Range; oY <= this.Range; oY++)
                            {
                                if (oX == 0 && oY == 0)
                                    continue;

                                int fX = x + oX;
                                int fY = y + oY;

                                if (fX < 0 || fX >= width ||
                                    fY < 0 || fY >= height)
                                    continue;

                                if (states[fX, fY] == stateCheck)
                                {
                                    count++;
                                }
                            }
                        }

                        if (count < this.CountMin || count > this.CountMax)
                            continue;

                        states[x, y] = this.State;
                    }
                }
            }

        }

        public class AreaFillRule : AreaRule
        {
            public int TileThresholdMin { get; private set; }
            public int TileThresholdMax { get; private set; }
            public int CountState { get; private set; }
            public int TargetState { get; private set; }


            public AreaFillRule(int tileThresholdMin, int tileThresholdMax, int countState, int targetState)
            {
                this.TileThresholdMin = tileThresholdMin;
                this.TileThresholdMax = tileThresholdMax;
                this.CountState = countState;
                this.TargetState = targetState;
            }


            public override void Apply(Random random, float[,] values, int[,] states)
            {
                int width = values.GetLength(0);
                int height = values.GetLength(1);

                Queue<Int2> toCheckTiles = new Queue<Int2>();
                List<Int2> activeTiles = new List<Int2>();

                bool[,] checkedTiles = new bool[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (checkedTiles[x, y])
                            continue;

                        checkedTiles[x, y] = true;

                        if (states[x, y] == this.CountState)
                        {
                            toCheckTiles.Enqueue(new Int2(x, y));
                        }

                        while (toCheckTiles.Count > 0)
                        {
                            Int2 tile = toCheckTiles.Dequeue();
                            checkedTiles[tile.X, tile.Y] = true;

                            activeTiles.Add(tile);

                            for (int aY = -1; aY <= 1; aY++)
                            {
                                for (int aX = -1; aX <= 1; aX++)
                                {
                                    if (aX != 0 && aY != 0)
                                        continue;

                                    int fX = tile.X + aX;
                                    int fY = tile.Y + aY;

                                    if (!this.IsValidTile(states, fX, fY))
                                        continue;

                                    if (checkedTiles[fX, fY])
                                        continue;

                                    if (states[fX, fY] == this.CountState)
                                    {
                                        toCheckTiles.Enqueue(new Int2(fX, fY));
                                        checkedTiles[fX, fY] = true;
                                    }
                                }
                            }
                        }

                        if (activeTiles.Count > this.TileThresholdMax)
                        {
                            activeTiles.Clear();
                            toCheckTiles.Clear();
                        }

                        if (activeTiles.Count > 0)
                        {
                            foreach (var tile in activeTiles)
                            {
                                states[tile.X, tile.Y] = this.TargetState;
                            }

                            activeTiles.Clear();
                            toCheckTiles.Clear();
                        }
                    }
                }
            }
        }

        public class AreaBorderRule : AreaRule
        {
            public int BorderStart { get; set; }
            public int BorderMin { get; private set; }
            public int BorderMax { get; private set; }
            public int State { get; private set; }


            public AreaBorderRule(int borderStart, int borderMin, int borderMax, int state)
            {
                this.BorderStart = borderStart;
                this.BorderMin = borderMin.Min(borderMax);
                this.BorderMax = borderMax.Max(borderMin);
                this.State = state;
            }


            public override void Apply(Random random, float[,] values, int[,] states)
            {
                int width = values.GetLength(0);
                int height = values.GetLength(1);

                int length = width.Max(height) - (this.BorderStart + this.BorderStart);


                for (int i = 0; i < length; i++)
                {
                    // Left
                    int borderSize = random.Next(this.BorderMin, this.BorderMax);
                    for (int b = 0; b < borderSize; b++)
                    {
                        if (this.BorderStart + this.BorderStart + i >= height)
                            break;

                        states[this.BorderStart + b, this.BorderStart + i] = this.State;
                    }

                    // Right
                    borderSize = random.Next(this.BorderMin, this.BorderMax);
                    for (int b = 0; b < borderSize; b++)
                    {
                        if (this.BorderStart + this.BorderStart + i >= height)
                            break;

                        states[width - 1 - this.BorderStart - b, this.BorderStart + i] = this.State;
                    }

                    // Bottom
                    borderSize = random.Next(this.BorderMin, this.BorderMax);
                    for (int b = 0; b < borderSize; b++)
                    {
                        if (this.BorderStart + this.BorderStart + i >= width)
                            break;

                        states[this.BorderStart + i, this.BorderStart + b] = this.State;
                    }

                    // Top
                    borderSize = random.Next(this.BorderMin, this.BorderMax);
                    for (int b = 0; b < borderSize; b++)
                    {
                        if (this.BorderStart + this.BorderStart + i >= width)
                            break;

                        states[this.BorderStart + i, height - 1 - this.BorderStart - b] = this.State;
                    }
                }

            }
        }

        public class AreaConnector : AreaRule
        {
            public int PathRadius { get; private set; }
            public int PathState { get; private set; }


            public AreaConnector(int pathRadius, int pathState)
            {
                this.PathRadius = pathRadius;
                this.PathState = pathState;
            }


            public override void Apply(Random random, float[,] values, int[,] states)
            {
                List<Area> notConnected = this.BuildAreas(states);

                if (notConnected.Count == 0)
                    return;

                Area mainArea = notConnected.OrderBy(a => -a.Tiles.Count).First();

                List<Area> connected = new List<Area>();
                connected.Add(mainArea);
                notConnected.Remove(mainArea);

                while (notConnected.Count > 0)
                {
                    Int2 connectedTile = connected[0].Borders[0];
                    Int2 notConnectedTile = notConnected[0].Borders[0];
                    Area notConnectedArea = notConnected[0];

                    int bestDistance = connectedTile.ManhattanDistance(notConnectedTile);

                    foreach (var area in connected)
                    {
                        foreach (var tile in area.Borders)
                        {
                            foreach (var notArea in notConnected)
                            {
                                foreach (var notTile in notArea.Borders)
                                {
                                    int distance = notTile.ManhattanDistance(tile);

                                    if (distance < bestDistance)
                                    {
                                        connectedTile = tile;
                                        notConnectedTile = notTile;
                                        notConnectedArea = notArea;
                                        bestDistance = distance;
                                    }
                                }
                            }
                        }
                    }

                    this.CreatePath(states, connectedTile, notConnectedTile);

                    notConnected.Remove(notConnectedArea);
                    connected.Add(notConnectedArea);
                }
            }

            void CreatePath(int[,] states, Int2 a, Int2 b)
            {
                int x = a.X;
                int y = a.Y;

                int dX = b.X - a.X;
                int dY = b.Y - a.Y;

                int dx1 = dX < 0 ? -1 : dX > 0 ? 1 : 0,
                    dy1 = dY < 0 ? -1 : dY > 0 ? 1 : 0,
                    dx2 = dX < 0 ? -1 : dX > 0 ? 1 : 0,
                    dy2 = 0;

                int max = Math.Abs(dX);
                int min = Math.Abs(dY);

                if (max < min)
                {
                    max = Math.Abs(dY);
                    min = Math.Abs(dX);
                    dy2 = dY < 0 ? -1 : dY > 0 ? 1 : 0;
                    dx2 = 0;
                }

                int add = max / 2;
                for (int i = 0; i <= max; i++)
                {
                    for (int rX = -this.PathRadius; rX <= this.PathRadius; rX++)
                    {
                        for (int rY = -this.PathRadius; rY <= this.PathRadius; rY++)
                        {
                            int fX = x + rX;
                            int fY = y + rY;

                            if (!this.IsValidTile(states, fX, fY))
                                continue;

                            states[fX, fY] = this.PathState;
                        }
                    }

                    add += min;
                    if (!(add < max))
                    {
                        add -= max;
                        x += dx1;
                        y += dy1;
                    }
                    else
                    {
                        x += dx2;
                        y += dy2;
                    }
                }
            }

            List<Area> BuildAreas(int[,] states)
            {
                List<Area> toReturn = new List<Area>();

                Dictionary<Area, List<Int2>> floodFill = new Dictionary<Area, List<Int2>>();

                int width = states.GetLength(0);
                int height = states.GetLength(1);

                Queue<Int2> toCheckTiles = new Queue<Int2>();
                List<Int2> activeAreaTiles = new List<Int2>();

                bool[,] checkedTiles = new bool[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (checkedTiles[x, y])
                            continue;

                        if (!this.RadiusCheck(states, x, y))
                            continue;

                        checkedTiles[x, y] = true;

                        if (states[x, y] == this.PathState)
                        {
                            toCheckTiles.Enqueue(new Int2(x, y));
                        }

                        while (toCheckTiles.Count > 0)
                        {
                            Int2 tile = toCheckTiles.Dequeue();
                            checkedTiles[tile.X, tile.Y] = true;

                            activeAreaTiles.Add(tile);

                            for (int aY = -1; aY <= 1; aY++)
                            {
                                for (int aX = -1; aX <= 1; aX++)
                                {
                                    if (aX == 0 && aY == 0)
                                        continue;

                                    int fX = tile.X + aX;
                                    int fY = tile.Y + aY;

                                    if (!this.IsValidTile(states, fX, fY))
                                    {
                                        continue;
                                    }

                                    if (checkedTiles[fX, fY])
                                        continue;

                                    if (this.RadiusCheck(states, fX, fY))
                                    {
                                        toCheckTiles.Enqueue(new Int2(fX, fY));
                                        checkedTiles[fX, fY] = true;
                                    }
                                    else
                                    {
                                        // Border Tile...?
                                    }
                                }
                            }
                        }

                        if (activeAreaTiles.Count > 0)
                        {
                            Area area = new Area(activeAreaTiles);
                            toReturn.Add(area);
                            floodFill.Add(area, new List<Int2>(area.Tiles));

                            activeAreaTiles = new List<Int2>();
                            //foreach (var tile in activeTiles)
                            //{
                            //    states[tile.X, tile.Y] = this.TargetState;
                            //}

                            //activeTiles.Clear();
                            //toCheckTiles.Clear();
                        }
                    }
                }

                // Flood Fill
                bool fillSuccess = false;
                do
                {
                    fillSuccess = false;
                    foreach (var area in floodFill)
                    {
                        List<Int2> add = new List<Int2>();

                        fillSuccess |= fillSuccess || area.Value.Count > 0;

                        foreach (var tile in area.Value)
                        {
                            for (int aY = -1; aY <= 1; aY++)
                            {
                                for (int aX = -1; aX <= 1; aX++)
                                {
                                    if (aX == 0 && aY == 0)
                                        continue;

                                    int fX = tile.X + aX;
                                    int fY = tile.Y + aY;

                                    if (!this.IsValidTile(states, fX, fY))
                                        continue;

                                    bool isContained = checkedTiles[fX, fY];
                                    if (isContained)
                                        continue;

                                    if (states[fX, fY] == this.PathState)
                                    {
                                        checkedTiles[fX, fY] = true;
                                        add.Add(new Int2(fX, fY));
                                    }
                                }
                            }
                        }

                        area.Value.Clear();
                        area.Value.AddRange(add);
                        area.Key.Tiles.AddRange(add);
                    }
                } while (fillSuccess);

                // Create Border Tiles
                foreach (var area in toReturn)
                {
                    List<Int2> borderTiles = new List<Int2>();

                    foreach (var tile in area.Tiles)
                    {
                        bool next = false;
                        for (int aY = -1; aY <= 1; aY++)
                        {
                            for (int aX = -1; aX <= 1; aX++)
                            {
                                if (aX == 0 && aY == 0)
                                    continue;

                                int fX = tile.X + aX;
                                int fY = tile.Y + aY;

                                if (!this.IsValidTile(states, fX, fY))
                                    continue;

                                if (states[fX, fY] != this.PathState)
                                {
                                    borderTiles.Add(tile);
                                    next = true;
                                    break;
                                }
                            }

                            if (next)
                                break;
                        }
                    }

                    area.Borders.AddRange(borderTiles);
                }

                return toReturn;
            }

            bool RadiusCheck(int[,] states, int stateX, int stateY)
            {
                for (int rX = -this.PathRadius; rX <= this.PathRadius; rX++)
                {
                    for (int rY = -this.PathRadius; rY <= this.PathRadius; rY++)
                    {
                        int fX = stateX + rX;
                        int fY = stateY + rY;

                        if (!this.IsValidTile(states, fX, fY))
                            continue;

                        if (states[fX, fY] != this.PathState)
                            return false;
                    }
                }

                return true;
            }
        }
    }
}
