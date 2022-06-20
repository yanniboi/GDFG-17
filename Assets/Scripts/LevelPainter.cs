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

    private void Start()
    {
        this.FetchTiles();
        this.PaintTiles();
    }

    private void FetchTiles()
    {
        string data = File.ReadAllText(LevelStorage.LevelStorageCurrentLevel);
        this.Chunks = JsonUtility.FromJson<LevelData>(data).Chunks;
    }

    private void PaintTiles()
    {
        Debug.Log($"PAINT {Chunks.Count}");

        foreach (LevelChunk chunk in this.Chunks)
        {

            Tilemap tilemap = GameObject.Instantiate(this.Prefab);
            tilemap.transform.SetParent(this.TileMapRoot.transform, false);
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
