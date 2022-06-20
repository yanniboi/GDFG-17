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
        this._tilemap = this.GetComponent<Tilemap>();
        this._shadowCaster = this.GetComponent<ShadowCaster2DTileMap>();
    }

    private void Update()
    {
        if (this._currentTile == default)
        {
            return;
        }
        this._beamTime += Time.deltaTime;

        if (this._beamTime >= this.DestroyTime)
        {
            this.DestroyTile(this._currentTile);
        }

    }

    private void StartBeam(Vector3 tile)
    {
        var tilepos = this._tilemap.WorldToCell(tile);

        if (tilepos != this._currentTile)
        {
            this.ColorTile(this._currentTile, Color.white);
            this._beamTime = 0;
        }

        this.ColorTile(tilepos, Color.red);
        this._currentTile = tilepos;
    }

    private void StopBeam()
    {
        this.ColorTile(this._currentTile, Color.white);
        this._currentTile = default;
        this._beamTime = 0;
    }

    /**
     * This will clear tiles from the tilemap and then update the shadows.
     */
    private void DestroyTile(Vector3Int tile)
    {
        this._tilemap.SetTile(tile, null);
        this.StartCoroutine(nameof(UpdateShadows));
    }

    private void ColorTile(Vector3Int tile, Color color)
    {
        this._tilemap.SetTileFlags(tile, TileFlags.None);
        this._tilemap.SetColor(tile, color);
    }

    private IEnumerator UpdateShadows()
    {
        yield return null;
        this._shadowCaster.DestroyAllChildren();
        this._shadowCaster.Generate();
    }

    private void OnEnable()
    {
        LazerGun.OnHitTile += this.StartBeam;
        LazerGun.OnNoHit += this.StopBeam;
    }

    private void OnDisable()
    {
        LazerGun.OnHitTile -= this.StartBeam;
        LazerGun.OnNoHit -= this.StopBeam;
    }
}
