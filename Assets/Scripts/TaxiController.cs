using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaxiController : MonoBehaviour
{
    public static event Action OnPickup, OnDropoff;

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

    [SerializeField] private int _detectorCount = 3;
    [SerializeField] [Range(0f, 1f)] private float _bouncyness = 0.5f;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    [SerializeField] private ParticleSystem particleSystemOnCollision;
    public ParticleSystem ParticleSystemOnCollision { get { return this.particleSystemOnCollision; } }


    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    private bool _colUp, _colRight, _colDown, _colLeft;

    [Header("Passenger")]
    private bool _hasPassenger;
    [SerializeField] private CapsuleLauncher _launcher;

    private void Start()
    {
        this._sprite = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPaused())
        {
            return;
        }

        this.GetInput();
        this.CheckCollision();
        this.UpdateVisuals();
        this.Move();
    }

    private void CheckCollision()
    {
        // Generate ray ranges. 
        this.CalculateRayRanged();

        // The rest
        this._colUp = RunDetection(this._raysUp);
        this._colLeft = RunDetection(this._raysLeft);
        this._colRight = RunDetection(this._raysRight);
        this._colDown = RunDetection(this._raysDown);

        bool RunDetection(RayRange range)
        {
            return this.EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, this._detectionRayLength, this._groundLayer));
        }
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(this.transform.position + this._characterBounds.center, this._characterBounds.size);

        this._raysDown = new RayRange(b.min.x + this._rayBuffer, b.min.y, b.max.x - this._rayBuffer, b.min.y, Vector2.down);
        this._raysUp = new RayRange(b.min.x + this._rayBuffer, b.max.y, b.max.x - this._rayBuffer, b.max.y, Vector2.up);
        this._raysLeft = new RayRange(b.min.x, b.min.y + this._rayBuffer, b.min.x, b.max.y - this._rayBuffer, Vector2.left);
        this._raysRight = new RayRange(b.max.x, b.min.y + this._rayBuffer, b.max.x, b.max.y - this._rayBuffer, Vector2.right);
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < this._detectorCount; i++)
        {
            var t = (float)i / (this._detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void GetInput()
    {
        this._inputHorizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(this._inputHorizontal) > 0.05f)
        {
            // _sprite.color = Color.green;
            this._speedHorizontal += this._inputHorizontal * this._acceleration * Time.deltaTime;
            this._speedHorizontal = Mathf.Clamp(this._speedHorizontal, -this._maxSpeed, this._maxSpeed);
            this.transform.localScale = new Vector3(this._inputHorizontal / Mathf.Abs(this._inputHorizontal), 1f, 1f);

        }
        else
        {
            // _sprite.color = Color.yellow;
            this._speedHorizontal = Mathf.MoveTowards(this._speedHorizontal, 0, this._deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(this._speedHorizontal) <= 0.05f)
        {
            // _sprite.color = Color.white;
        }

        if (this._speedHorizontal > 0 && this._colRight || this._speedHorizontal < 0 && this._colLeft)
        {
            // Don't walk through walls
            this._speedHorizontal = -this._speedHorizontal * this._bouncyness;
        }


        this._inputVertical = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(this._inputVertical) > 0.05f)
        {
            // _sprite.color = Color.green;
            this._speedVertical += this._inputVertical * this._acceleration * Time.deltaTime;
            this._speedVertical = Mathf.Clamp(this._speedVertical, -this._maxSpeed, this._maxSpeed);
        }
        else
        {
            // _sprite.color = Color.yellow;
            this._speedVertical = Mathf.MoveTowards(this._speedVertical, 0, this._deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(this._speedVertical) <= 0.05f)
        {
            // _sprite.color = Color.white;
        }

        if (this._speedVertical > 0 && this._colUp || this._speedVertical < 0 && this._colDown)
        {
            // Don't walk through walls
            this._speedVertical = -this._speedVertical * this._bouncyness;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this._hasPassenger)
            {
                Dropoff();
                _launcher.Launch();
            }
        }
    }



    private int _freeColliderIterations = 10;
    private void Move()
    {
        var pos = this.transform.position;
        var move = new Vector3(this._speedHorizontal, this._speedVertical) * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, this._characterBounds.size, 0, this._groundLayer);
        if (!hit)
        {
            this.transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = this.transform.position;
        for (int i = 1; i < this._freeColliderIterations; i++)
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / this._freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, this._characterBounds.size, 0, this._groundLayer))
            {
                this.transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1)
                {
                    this._speedHorizontal = -this._speedHorizontal * this._bouncyness;
                    this._speedVertical = -this._speedVertical * this._bouncyness;
                    // if (_speedVertical < 0) _speedVertical = 0;
                    // var dir = transform.position - hit.transform.position;
                    // transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    private void UpdateVisuals()
    {
        if (this._colDown || this._colLeft || this._colRight || this._colUp)
            this.ParticleSystemOnCollision.Play();
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position + this._characterBounds.center, this._characterBounds.size);

        // Rays
        if (!Application.isPlaying)
        {
            this.CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { this._raysUp, this._raysRight, this._raysDown, this._raysLeft })
            {
                foreach (var point in this.EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * this._detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(this._speedHorizontal, this._speedVertical) * Time.deltaTime;
        Gizmos.DrawWireCube(this.transform.position + this._characterBounds.center + move, this._characterBounds.size);
    }

    public struct RayRange
    {
        public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
        {
            this.Start = new Vector2(x1, y1);
            this.End = new Vector2(x2, y2);
            this.Dir = dir;
        }

        public readonly Vector2 Start, End, Dir;
    }

    public bool HasPassenger()
    {
        return this._hasPassenger;
    }

    public void Pickup()
    {
        this._hasPassenger = true;
        OnPickup?.Invoke();
    }

    private void Dropoff()
    {
        this._hasPassenger = false;
        OnDropoff?.Invoke();
    }
}
