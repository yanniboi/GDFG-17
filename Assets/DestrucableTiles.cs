using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestrucableTiles : MonoBehaviour
{
    public float DestroyTime = 2f;
    
    private Tilemap _tilemap;
    private ShadowCaster2DTileMap _shadowCaster;

    private Vector3Int _currentTile;
    private float _beamTime = 0f;
    
    
    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _shadowCaster = GetComponent<ShadowCaster2DTileMap>();
    }

    private void Update()
    {
        if (_currentTile == default)
        {
            return;
        }
        _beamTime += Time.deltaTime;

        if (_beamTime >= DestroyTime)
        {
            DestroyTile(_currentTile);
        }
        
    }

    private void StartBeam(Vector3 tile)
    {
        var tilepos = _tilemap.WorldToCell(tile);

        if (tilepos != _currentTile)
        {
            ColorTile(_currentTile, Color.white);
            _beamTime = 0;
        }

        ColorTile(tilepos, Color.red);
        _currentTile = tilepos;
    }
    
    private void StopBeam()
    {
        ColorTile(_currentTile, Color.white);
        _currentTile = default;
        _beamTime = 0;
    }
    
    /**
     * This will clear tiles from the tilemap and then update the shadows.
     */
    private void DestroyTile(Vector3Int tile)
    {
        _tilemap.SetTile(tile, null);
        StartCoroutine(nameof(UpdateShadows));
    }

    private void ColorTile(Vector3Int tile, Color color)
    {
        _tilemap.SetTileFlags(tile, TileFlags.None);
        _tilemap.SetColor(tile, color);
    }
    
    private IEnumerator UpdateShadows()
    {
        yield return null;
        _shadowCaster.DestroyAllChildren();
        _shadowCaster.Generate();
    }
    
    private void OnEnable()
    {
        LazerGun.OnHitTile += StartBeam;
        LazerGun.OnNoHit += StopBeam;
    }

    private void OnDisable()
    {
        LazerGun.OnHitTile -= StartBeam;
        LazerGun.OnNoHit -= StopBeam;
    }
}
