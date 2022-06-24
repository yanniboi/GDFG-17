using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelPainter : MonoBehaviour
{

    [SerializeField] private Tilemap prefab;
    public Tilemap Prefab { get { return prefab; } }

    [SerializeField] private Grid tileMapRoot;
    public Grid TileMapRoot { get { return tileMapRoot; } }



    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private ShadowCaster2DTileMap _shadowCaster;

    private string _seed;
    private List<LevelChunk> Chunks { get; set; }
    private LevelData LevelData { get; set; }

    private Vector3 SpawnOffset { get; set; }

    private void Start()
    {
        this.FetchTiles();
        this.PaintTiles();
    }

    private void FetchTiles()
    {
        string data = File.ReadAllText(LevelStorage.LevelStorageCurrentLevel);
        this.LevelData = JsonUtility.FromJson<LevelData>(data);
        this.Chunks = this.LevelData.Chunks;

        Vector3 offset = new Vector3();

        for (int i = 0; i < this.Chunks.Count; i++)
        {
            int[] states1D = this.Chunks[i].States;

            int tilesX = this.LevelData.AreaTilesX;
            int tilesY = this.LevelData.AreaTilesY;

            Debug.Log(i);

            int[,] states = new int[tilesX, tilesY];

            for (var x = 0; x < tilesX; x++)
            {
                for (var y = 0; y < tilesY; y++)
                {
                    states[x, y] = states1D[y * tilesX + x];
                }
            }

            bool success = true;
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    success = true;

                    if (states[x, y] != 0)
                    {
                        success = false;
                        continue;
                    }

                    for (int oX = -1; oX <= 1; oX++)
                    {
                        for (int oY = -1; oY <= 1; oY++)
                        {
                            int fX = x + oX;
                            int fY = y + oY;

                            if (fX < 0 || fX >= tilesX || fY < 0 || fY >= tilesY)
                                continue;

                            if (states[fX, fY] != 0)
                            {
                                success = false;
                                break;
                            }
                        }

                        if (!success)
                            break;
                    }

                    if (success)
                    {
                        int chunkX = i % this.LevelData.Width;
                        int chunkY = i / this.LevelData.Width;

                        offset.x = -(chunkX * this.LevelData.AreaTilesX + x)  *2;
                        offset.y = -(chunkY * this.LevelData.AreaTilesY + y) * 2;

                        Debug.Log($"YES {offset.x}///{offset.y}");
                        break;
                    }
                }

                if (success)
                {
                    i = this.Chunks.Count;
                    break;
                }
            }
        }

        this.SpawnOffset = offset;
        Debug.Log($"Spawn Offset = {this.SpawnOffset}");
    }

    private void PaintTiles()
    {
        Debug.Log($"PAINT {Chunks.Count}");

        foreach (LevelChunk chunk in this.Chunks)
        {
            Tilemap tilemap = GameObject.Instantiate(this.Prefab);
            tilemap.transform.SetParent(this.TileMapRoot.transform, false);
            tilemap.transform.position += this.SpawnOffset;
            this.PlaceTiles(tilemap, chunk);
        }
    }

    private IEnumerator UpdateShadows()
    {
        yield return null;
        this._shadowCaster.DestroyAllChildren();
        this._shadowCaster.Generate();
    }

    private void PlaceTiles(Tilemap tilemap, LevelChunk chunk)
    {
        foreach (var position in chunk.Tiles)
        {
            tilemap.SetTile(this._tilemap.WorldToCell(position), this._tile);
        }
    }

    private void PlaceTile(Vector3 position)
    {
        this._tilemap.SetTile(this._tilemap.WorldToCell(position), this._tile);
    }

    private void RemoveTile(Vector3 position)
    {
        this._tilemap.SetTile(this._tilemap.WorldToCell(position), null);
    }
}
