using System.Collections.Generic;
using System.IO;
using System.Threading;
using AreaGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int width = 3;
    public int Width { get { return width; } }

    [SerializeField] private int height = 3;
    public int Height { get { return height; } }


    [SerializeField] private int areaTilesX = 30;
    public int AreaTilesX { get { return areaTilesX; } }

    [SerializeField] private int areaTilesY = 30;
    public int AreaTilesY { get { return areaTilesY; } }


    private string _seed;

    public LoadingState State { get; set; }

    AreaGenerator2D AreaGenerator2D { get; set; }

    private void Update()
    {
        if (this.State?.IsDone == true)
        {
            this.FinishGenerate();
            
            // @Fresch why is this important?
            // this.State = null;
        }
    }

    public void Generate(string seed)
    {
        _seed = seed;
        this.AreaGenerator2D = new AreaGenerator2D(_seed.GetHashCode());

        Thread generator = new Thread(this.DoGenerate);
        generator.Name = "Generator";
        generator.IsBackground = true;
        generator.Start();
    }

    private void DoGenerate()
    {
        this.State = new LoadingState();

        this.GenerateTiles();
    }

    private void FinishGenerate()
    {
        LevelTiles tiles = new LevelTiles();
        tiles.tiles = State.Tiles;
        string data = JsonUtility.ToJson(tiles);
        LevelStorage.CheckStoragePath();
        File.WriteAllText(LevelStorage.LevelStorageCurrentLevel, data);
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

    public class LoadingState
    {
        public List<Vector3> Tiles { get; set; }
        public bool IsDone { get; set; }
    }
}