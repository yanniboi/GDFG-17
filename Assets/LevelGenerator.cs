using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private ShadowCaster2DTileMap _shadowCaster;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        List<Vector3> positions = GetPositions();
        foreach (Vector3 position in positions)
        {
            PlaceTile(position);
        }
        StartCoroutine(nameof(UpdateShadows));
    }
    
    private IEnumerator UpdateShadows()
    {
        yield return null;
        _shadowCaster.DestroyAllChildren();
        _shadowCaster.Generate();
    }

    private List<Vector3> GetPositions()
    {
        var list = new List<Vector3>();
        
        list.Add(new Vector3(0, 0, 0));
        list.Add(new Vector3(1, 1, 0));
        list.Add(new Vector3(1, 0, 0));
        list.Add(new Vector3(0, 1, 0));
        list.Add(new Vector3(0, 2, 0));
        
        return list;
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
