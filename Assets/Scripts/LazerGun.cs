using System;
using UnityEngine;

public class LazerGun : MonoBehaviour
{
    public static event Action<Vector3> OnHitTile;
    public static event Action OnNoHit;

    public LineRenderer _line;
    public float lazerdistance;
    public LayerMask LayerMask;

    private Vector3 _lazerTarget;
    private Vector3 _lazerHitPoint;

    void Update()
    {
        this.GetTarget();
        this.NotifyTiles();
        this.ShootLazer();
    }

    private void GetTarget()
    {
        // Set to zero by default so we dont shoot unless we press a key.
        this._lazerTarget = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Take ship orientation into account
            Vector2 direction = Vector2.right * this.gameObject.transform.parent.transform.localScale.x;

            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, this.lazerdistance, this.LayerMask);
            if (hit.collider != null)
            {
                this._lazerHitPoint = hit.point;
                this._lazerTarget = (hit.point) - (Vector2)this.transform.position;
            }
            else
            {
                this._lazerHitPoint = default;
                this._lazerTarget = direction * this.lazerdistance;
            }

            // Debug
            Debug.DrawRay(this.transform.position, direction * this.lazerdistance, Color.green);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            this._lazerHitPoint = default;
            OnNoHit?.Invoke();
        }
    }

    private void NotifyTiles()
    {
        if (this._lazerHitPoint != default)
        {
            Vector2 direction = Vector2.right * this.gameObject.transform.parent.transform.localScale.x;
            var tile = Vector3.zero;
            tile.x = this._lazerHitPoint.x + 0.01f * direction.x;
            tile.y = this._lazerHitPoint.y + 0.01f * direction.y;
            OnHitTile?.Invoke(tile);
        }
        else
        {
            OnNoHit?.Invoke();
        }
    }

    private void ShootLazer()
    {
        // Line position is relative to the line origin and so should always be positive.
        Vector3 linePosition = this._lazerTarget * this._lazerTarget.normalized.x;
        this._line.SetPosition(1, linePosition);
    }

}
