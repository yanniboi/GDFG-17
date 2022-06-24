using AreaGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using static AreaGeneration.AreaGenerator2D;

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
    AreaConfig FinalConfig { get; set; }

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
        this.FinalConfig = new AreaGenerator2D.AreaConfig(this.Width, this.Height, this.AreaTilesX, this.AreaTilesY, 2, 0)
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

        this.AreaGenerator2D.Generate(this.FinalConfig, out int[,][,] chunkValues, out int[,] stateValues);

        List<Vector3[]> chunks = new List<Vector3[]>();
        List<int[,]> chunkStates = new List<int[,]>();

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

                            chunkTiles.Add(new Vector3(fX, fY));
                        }
                    }
                }

                chunkStates.Add(chunkValues[cX, cY]);
                chunks.Add(chunkTiles.ToArray());
            }
        }

        this.State.States = chunkStates.ToArray();
        this.State.Chunks = chunks.ToArray();
        this.State.IsDone = true;
    }

    private void FinishGenerate()
    {
        LevelData levelData = new LevelData();

        List<LevelChunk> chunks = new List<LevelChunk>();
        for (int i = 0; i < this.State.Chunks.Length; i++)
        {
            int width = this.State.States[i].GetLength(0);
            int height = this.State.States[i].GetLength(1);
            int[] states = new int[width * height];

            int idx = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    states[idx++] = this.State.States[i][x, y];
                }
            }

            //LevelChunk chunk = new LevelChunk(null);
            LevelChunk chunk = new LevelChunk(this.State.Chunks[i].ToList()); // { States = this.State.States[i] };
            chunk.States = states;

            chunks.Add(chunk);
        }

        levelData.Chunks = chunks; /*this.State.Chunks.Select(chunk => new LevelChunk(chunk.ToList())).ToList();*/
        levelData.AreaTilesX = this.FinalConfig.AreaTilesX;
        levelData.AreaTilesY = this.FinalConfig.AreaTilesY;
        levelData.Width = this.FinalConfig.Width;
        levelData.Height = this.FinalConfig.Height;

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
        public int[][,] States { get; set; }
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
