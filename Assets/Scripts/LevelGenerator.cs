using AreaGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Action<float> OnProgress { get; set; }

    [SerializeField] private int width = 3;
    public int Width { get { return this.width; } }

    [SerializeField] private int height = 3;
    public int Height { get { return this.height; } }


    [SerializeField] private int areaTilesX = 30;
    public int AreaTilesX { get { return this.areaTilesX; } }

    [SerializeField] private int areaTilesY = 30;
    public int AreaTilesY { get { return this.areaTilesY; } }


    private string Seed { get; set; }

    public LoadingState State { get; set; }

    AreaGenerator2D AreaGenerator2D { get; set; }

    private void Update()
    {
        if (this.State?.IsDone == true)
        {
            this.FinishGenerate();
            this.State = null; // Prevents that the level is built again and again and again and again
        }
    }

    public void Generate(string seed)
    {
        if (this.State != null)
            return;

        this.Seed = seed;
        this.AreaGenerator2D = new AreaGenerator2D(this.Seed.GetHashCode());

        this.AreaGenerator2D.OnGenerationStart += this.AreaGenerator2D_OnGenerationStart;
        this.AreaGenerator2D.OnGenerationProgress += this.AreaGenerator2D_OnGenerationProgress;

        this.State = new LoadingState();

        Thread generator = new Thread(this.DoGenerate);
        generator.Name = "Generator";
        generator.IsBackground = true;
        generator.Start();
    }

    private void DoGenerate()
    {
        this.GenerateTiles();

        // Unsubscribe
        this.AreaGenerator2D.OnGenerationStart -= this.AreaGenerator2D_OnGenerationStart;
        this.AreaGenerator2D.OnGenerationProgress -= this.AreaGenerator2D_OnGenerationProgress;
    }

    private void GenerateTiles()
    {
        List<Vector3> tiles = new List<Vector3>();

        AreaGenerator2D.AreaConfig config = new AreaGenerator2D.AreaConfig(this.Width, this.height, this.AreaTilesX, this.AreaTilesY)
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

        int[,] tileStates = this.AreaGenerator2D.Generate(config);

        int width = tileStates.GetLength(0);
        int height = tileStates.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (tileStates[x, y] == 1)
                    tiles.Add(new Vector3(x, y));
            }
        }

        this.State.Tiles = tiles;
        this.State.IsDone = true;
    }

    private void FinishGenerate()
    {
        LevelTiles tiles = new LevelTiles();

        tiles.tiles = this.State.Tiles;

        string data = JsonUtility.ToJson(tiles);

        LevelStorage.CheckStoragePath();

        File.WriteAllText(LevelStorage.LevelStorageCurrentLevel, data);
    }

    #region Event Methods
    private void AreaGenerator2D_OnGenerationStart(int progressMax)
    {
        this.State?.SetProgressMax(progressMax);
    }

    private void AreaGenerator2D_OnGenerationProgress(int progress)
    {
        this.State?.SetProgress(progress);
    }
    #endregion


    public class LoadingState
    {
        public List<Vector3> Tiles { get; set; }
        public bool IsDone { get; set; }

        public int ProgressMax { get; private set; }
        public int ProgressCurrent { get; private set; }

        public void SetProgressMax(int value)
        {
            this.ProgressMax = value;
        }

        public void SetProgress(int value)
        {
            this.ProgressCurrent = value;
        }
    }
}
