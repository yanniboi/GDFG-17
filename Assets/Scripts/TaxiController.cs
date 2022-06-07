using System;
using UnityEngine;

public class TaxiController : MonoBehaviour
{
    private SpriteRenderer _sprite;
    
    [Header("Taxi Controls")]
    [SerializeField] private float _acceleration = 30;
    [SerializeField] private float _deceleration = 5;
    [SerializeField] private float _maxSpeed = 13;

    private float _inputHorizontal, _inputVertical;
    private float _speedHorizontal, _speedVertical;

    [Header("Collision")] 
    [SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

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
            // _sprite.color = Color.green;
            _speedHorizontal += _inputHorizontal * _acceleration * Time.deltaTime;
            _speedHorizontal = Mathf.Clamp(_speedHorizontal, -_maxSpeed, _maxSpeed);  
            transform.localScale = new Vector3(_inputHorizontal / Mathf.Abs(_inputHorizontal), 1f, 1f);

        }
        else           
        {
            // _sprite.color = Color.yellow;
            _speedHorizontal = Mathf.MoveTowards(_speedHorizontal, 0, _deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(_speedHorizontal) <= 0.05f)
        {
            // _sprite.color = Color.white;
        }
        
        _inputVertical = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(_inputVertical) > 0.05f)
        {
            // _sprite.color = Color.green;
            _speedVertical += _inputVertical * _acceleration * Time.deltaTime;
            _speedVertical = Mathf.Clamp(_speedVertical, -_maxSpeed, _maxSpeed);    
        }
        else           
        {
            // _sprite.color = Color.yellow;
            _speedVertical = Mathf.MoveTowards(_speedVertical, 0, _deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(_speedVertical) <= 0.05f)
        {
            // _sprite.color = Color.white;
        }

        
    }

    private void Move() {
        var pos = transform.position;
        var move = new Vector3(_speedHorizontal, _speedVertical) * Time.deltaTime;
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
