using UnityEngine;

public class LazerGun : MonoBehaviour
{
    public LineRenderer _line;
    public float lazerdistance;
    public LayerMask LayerMask;

    private Vector3 _lazerTarget;

    void Update()
    {
        GetTarget();
        ShootLazer();
    }

    private void GetTarget()
    {
        _lazerTarget = Vector3.zero;
        if (Input.GetKey(KeyCode.Space))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, lazerdistance, LayerMask);
            if (hit.collider != null)
            {
                _lazerTarget = hit.point - (Vector2) transform.position;
            }
            else
            {
                _lazerTarget = Vector3.right * lazerdistance;

            }
            
            Debug.DrawRay(transform.position, Vector3.right*lazerdistance, Color.green);
        }
    }
    
    private void ShootLazer()
    {
        _line.SetPosition(1, _lazerTarget);
    }
    
}
