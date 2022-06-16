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
        GetTarget();
        NotifyTiles();
        ShootLazer();
    }

    private void GetTarget()
    {
        // Set to zero by default so we dont shoot unless we press a key.
        _lazerTarget = Vector3.zero;
        if (Input.GetKey(KeyCode.Space))
        {
            // Take ship orientation into account
            Vector2 direction = Vector2.right * gameObject.transform.parent.transform.localScale.x;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lazerdistance, LayerMask);
            if (hit.collider != null)
            {
                _lazerHitPoint = hit.point;
                _lazerTarget = (hit.point) - (Vector2) transform.position;
            }
            else
            {
                _lazerHitPoint = default;
                _lazerTarget = direction * lazerdistance;
            }

            // Debug
            Debug.DrawRay(transform.position, direction*lazerdistance, Color.green);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _lazerHitPoint = default;
            OnNoHit?.Invoke();
        }
    }
    
    private void NotifyTiles()
    {
        if (_lazerHitPoint != default)
        {
            Vector2 direction = Vector2.right * gameObject.transform.parent.transform.localScale.x;
            var tile = Vector3.zero;
            tile.x = _lazerHitPoint.x + 0.01f * direction.x;
            tile.y = _lazerHitPoint.y + 0.01f * direction.y;
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
        Vector3 linePosition = _lazerTarget * _lazerTarget.normalized.x;
        _line.SetPosition(1, linePosition);
    }
    
}
