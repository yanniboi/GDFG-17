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
        FetchTiles();
        PaintTiles();
    }

    private void FetchTiles()
    {
        string data = File.ReadAllText(LevelStorage.LevelStorageCurrentLevel);
        _tiles = JsonUtility.FromJson<LevelTiles>(data).tiles;
    }

    private void PaintTiles()
    {
        foreach (Vector3 position in _tiles)
        {
            PlaceTile(position);
        }

    }

    private IEnumerator UpdateShadows()
    {
        yield return null;
        _shadowCaster.DestroyAllChildren();
        _shadowCaster.Generate();
    }
    
    private void PlaceTile(Vector3 position)
    {
        _tilemap.SetTile(_tilemap.WorldToCell(position), _tile);
    }
    
    private void RemoveTile(Vector3 position)
    {
        _tilemap.SetTile(_tilemap.WorldToCell(position), null);
    }
}
