// Decompiled with JetBrains decompiler
// Type: MoreMountains.InfiniteRunnerEngine.Parallax
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE9AC18D-DD28-4E21-930D-E4B7F674B547
// Assembly location: /home/yanniboi/Games/Freschyboi/InfiniteRunnerAssembly.dll

using UnityEngine;

namespace InfiniteRunner
{
    public class Parallax : MonoBehaviour
    {
        public float ParallaxSpeed;
        public PossibleDirections ParallaxDirection;
        protected GameObject _clone;
        protected Vector3 _movement;
        protected Vector3 _initialPosition;
        protected Vector3 _newPosition;
        protected Vector3 _direction;
        protected float _width;

        protected virtual void Start()
        {
            var bounds = this.GetComponent<Collider2D>().bounds;
            if (this.ParallaxDirection == PossibleDirections.Left || this.ParallaxDirection == PossibleDirections.Right)
            {
                this._width = bounds.size.x;
                this._newPosition = new Vector3(this.transform.position.x + this._width, this.transform.position.y, this.transform.position.z);
            }
            if (this.ParallaxDirection == PossibleDirections.Up || this.ParallaxDirection == PossibleDirections.Down)
            {
                this._width = bounds.size.y;
                this._newPosition = new Vector3(this.transform.position.x, this.transform.position.y + this._width, this.transform.position.z);
            }
            if (this.ParallaxDirection == PossibleDirections.Forwards || this.ParallaxDirection == PossibleDirections.Backwards)
            {
                this._width = bounds.size.z;
                this._newPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + this._width);
            }
            switch (this.ParallaxDirection)
            {
                case PossibleDirections.Left:
                    this._direction = Vector3.left;
                    break;
                case PossibleDirections.Right:
                    this._direction = Vector3.right;
                    break;
                case PossibleDirections.Up:
                    this._direction = Vector3.up;
                    break;
                case PossibleDirections.Down:
                    this._direction = Vector3.down;
                    break;
                case PossibleDirections.Forwards:
                    this._direction = Vector3.forward;
                    break;
                case PossibleDirections.Backwards:
                    this._direction = Vector3.back;
                    break;
            }
            this._initialPosition = this.transform.position;
            this._clone = Instantiate(this.gameObject, this._newPosition, this.transform.rotation);
            Destroy(this._clone.GetComponent<Parallax>());
        }

        protected virtual void Update()
        {
            this._movement = this._direction * (this.ParallaxSpeed / 10f) * Time.deltaTime;
            this._clone.transform.Translate(this._movement);
            this.transform.Translate(this._movement);
            if (!this.ShouldResetPosition())
                return;
            this.transform.Translate(-this._direction * this._width);
            this._clone.transform.Translate(-this._direction * this._width);
        }

        protected virtual bool ShouldResetPosition()
        {
            switch (this.ParallaxDirection)
            {
                case PossibleDirections.Left:
                    return this.transform.position.x + (double)this._width < this._initialPosition.x;
                case PossibleDirections.Right:
                    return this.transform.position.x - (double)this._width > this._initialPosition.x;
                case PossibleDirections.Up:
                    return this.transform.position.y - (double)this._width > this._initialPosition.y;
                case PossibleDirections.Down:
                    return this.transform.position.y + (double)this._width < this._initialPosition.y;
                case PossibleDirections.Forwards:
                    return this.transform.position.z - (double)this._width > this._initialPosition.z;
                case PossibleDirections.Backwards:
                    return this.transform.position.z + (double)this._width < this._initialPosition.z;
                default:
                    return false;
            }
        }

        public enum PossibleDirections
        {
            Left,
            Right,
            Up,
            Down,
            Forwards,
            Backwards,
        }
    }
}
