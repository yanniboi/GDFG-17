using AreaGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        

        AreaGenerator2D.AreaConfig config = new AreaGenerator2D.AreaConfig(this.Width, this.Height, this.AreaTilesX, this.AreaTilesY, 1, 0)
                    .Set(
                    new AreaGenerator2D.TileRule(-10, .2f, .5f, 0),
                    new AreaGenerator2D.TileRule(.1f, 10f, .5f, 1))
                    .Set(
                    new AreaGenerator2D.AreaSpotRemover(0, 2, 1, 0),
                    new AreaGenerator2D.AreaBorderRule(0, 5, 8, 1), // Create Border
                                                                    // new AreaGenerator2D.AreaBorderRule(20, 5, 8, 1), // Create Ring
                    new AreaGenerator2D.AreaFillRule(0, 40, 0, 1),  // Convert small holes into walls
                    new AreaGenerator2D.AreaFillRule(0, 40, 1, 0), // Convert small walls into emptiness
                    new AreaGenerator2D.AreaConnector() // Make Sure... everything is connected
                    );

        this.AreaGenerator2D.Generate(config, out int[,][,] chunkValues, out int[,] stateValues);

        List <Vector3> tiles = new List<Vector3>();
        List<Vector3[]> chunks = new List<Vector3[]>();

        for (int cY = 0; cY < this.Height; cY++)
        {
            for (int cX = 0; cX < this.Width; cX++)
            {
                List<Vector3> chunkTiles = new List<Vector3>();

                for (int tY = 0; tY < this.AreaTilesY; tY++)
                {
                    for (int tX = 0; tX < this.AreaTilesX; tX++)
                    {
                        if (chunkValues[cX, cY][tX, tY] == 1)
                        {
                            int fX = cX * this.AreaTilesX + tX;
                            int fY = cY * this.AreaTilesY + tY;

                            if (chunkValues[cX, cY][tX, tY] != stateValues[fX, fY])
                            {
                                Debug.Log("???!!?!");
                            }

                            tiles.Add(new Vector3(fX, fY));
                            chunkTiles.Add(new Vector3(fX, fY));
                        }
                    }
                }

                chunks.Add(chunkTiles.ToArray());
            }
        }

        this.State.Tiles = tiles;
        this.State.Chunks = chunks.ToArray();
        this.State.IsDone = true;
    }

    private void FinishGenerate()
    {
        LevelData levelData = new LevelData();

        levelData.Chunks = this.State.Chunks.Select(chunk => new LevelChunk(chunk.ToList())).ToList();

        string data = JsonUtility.ToJson(levelData);

        Debug.Log($"Save Chunks {levelData.Chunks.Count} {data}");

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
        public Vector3[][] Chunks { get; set; }

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
