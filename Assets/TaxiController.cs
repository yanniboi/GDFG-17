using UnityEngine;

public class TaxiController : MonoBehaviour
{
    [Header("Taxi Controls")]
    [SerializeField] private float _acceleration = 30;
    [SerializeField] private float _deceleration = 5;
    [SerializeField] private float _maxSpeed = 13;

    private float _inputHorizontal, _inputVertical;
    private float _speedHorizontal, _speedVertical;

    [Header("Collision")] 
    [SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    
    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
    }

    private void GetInput()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(_inputHorizontal) > 0.05f)
        {
            _speedHorizontal += _inputHorizontal * _acceleration * Time.deltaTime;
            _speedHorizontal = Mathf.Clamp(_speedHorizontal, -_maxSpeed, _maxSpeed);    
        }
        else
        {
            _speedHorizontal = Mathf.MoveTowards(_speedHorizontal, 0, _deceleration * Time.deltaTime);
        }
        
        
        _inputVertical = Input.GetAxisRaw("Vertical");
    }

    private void Move() {
        var pos = transform.position;
        var move = new Vector3(_speedHorizontal, _inputVertical) * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit) {
            transform.position += move;
        }
    }

    private void OnDrawGizmos() {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);
    }

}
