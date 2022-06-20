using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelPainter : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private ShadowCaster2DTileMap _shadowCaster;

    private string _seed;
    private List<Vector3> _tiles;

    private void Start()
    {
        this.FetchTiles();
        this.PaintTiles();
    }

    private void FetchTiles()
    {
        string data = File.ReadAllText(LevelStorage.LevelStorageCurrentLevel);
        this._tiles = JsonUtility.FromJson<LevelTiles>(data).tiles;
    }

    private void PaintTiles()
    {
        foreach (Vector3 position in this._tiles)
        {
            this.PlaceTile(position);
        }

    }

    private IEnumerator UpdateShadows()
    {
        yield return null;
        this._shadowCaster.DestroyAllChildren();
        this._shadowCaster.Generate();
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
