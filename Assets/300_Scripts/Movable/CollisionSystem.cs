using System;
using HorrorPS1.HorrorPhysics;
using HorrorPS1.Settings;
using HorrorPS1.Tools;
using UnityEngine;

namespace HorrorPS1.Movable
{
    /// <summary>
    /// Associated with <see cref="CollisionSystem"/>
    /// for object collision calculs.
    /// </summary>
    public enum CollisionSystemType
    {
        Simple,
        Complex,
        Creature
    }

    /// <summary>
    /// Utility methods related to <see cref="CollisionSystemType"/>.
    /// </summary>
    internal static class CollisionSystemTypeExtensions
    {
        #region Generator
        /// <summary>
        /// Get a new <see cref="CollisionSystem"/> instance
        /// for this collision type, to be used for collision calculs.
        /// </summary>
        /// <param name="_collision">Collision system type to get <see cref="CollisionSystem"/> for.</param>
        /// <param name="_movable"><see cref="Movable"/> associated with this collision system.</param>
        /// <returns>New instance of a <see cref="CollisionSystem"/> for the given type.</returns>
        public static CollisionSystem Get(this CollisionSystemType _collision, Movable _movable)
        {
            switch (_collision)
            {
                case CollisionSystemType.Simple:
                    return new SimpleCollisionSystem(_movable);

                case CollisionSystemType.Complex:
                    return new ComplexCollisionSystem(_movable);

                case CollisionSystemType.Creature:
                    return new CreatureCollisionSystem(_movable);

                default:
                    throw new InvalidCollisionSystemTypeException();
            }
        }
        #endregion
    }

    /// <summary>
    /// Provides methods that determine how an object
    /// reacts to collisions and move in space.
    /// </summary>
    internal abstract class CollisionSystem
    {
        #region Global Members
        /// <summary>
        /// Maximum amount of recursive loop calculs
        /// to be used for collision performance.
        /// </summary>
        public const int MaxCollisionCalculRecursivity = 3;

        // -----------------------

        protected static readonly Buffer<RaycastHit> buffer = new Buffer<RaycastHit>(3);
        protected readonly Movable movable = null;

        /// <summary>
        /// Default empty buffer when no collision has been performed.
        /// </summary>
        public static Buffer<RaycastHit> DefaultBuffer
        {
            get
            {
                buffer.Clear();
                return buffer;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a collision system to be associated with a <see cref="Movable"/>.
        /// </summary>
        public CollisionSystem(Movable _movable)
        {
            movable = _movable;
        }
        #endregion

        #region Collision Calculs
        /// <summary>
        /// Performs collisions on associated <see cref="Movable"/>
        /// from a certain velocity, and move its rigidbody accordingly.
        /// </summary>
        /// <returns>All hits encountered the object had collision with.</returns>
        public abstract Buffer<RaycastHit> PerformCollisions(Vector3 _velocity);
        #endregion

        #region Additional Calculs
        /// <summary>
        /// Compute associated <see cref="Movable"/> velocity before collisions.
        /// </summary>
        public virtual void ComputeVelocity() { }

        /// <summary>
        /// Performs additional calculs before setting ground state.
        /// </summary>
        protected void SetGroundState(bool _isGrounded)
        {
            if (movable.UseGravity && !_isGrounded)
            {
                // Iterate over movement hits to find if one of these
                // can be considered as ground.
                for (int _i = 0; _i < buffer.Count; _i++)
                {
                    RaycastHit _hit = buffer[_i];
                    if (PhysicsUtility.IsGroundSurface(_hit.normal))
                    {                     
                        movable.groundNormal = _hit.normal;
                        _isGrounded = true;

                        break;
                    }
                }

                // If didn't hit ground during movement,
                // try to get it with two casts:
                //  • A raycast from collider bottom,
                //  • A Shapecast if the previous raycast failed.
                //
                // Necessary when movement magnitude is inferior to default contact offset.
                //
                // If using a sphere or a capsule collider, cast can retrieve an obstacle
                // different than the ground when against a slope, that's why a raycast
                // from bottom center is needed.
                if (!_isGrounded)
                {
                    if ((movable.collider.Raycast(-movable.groundNormal, out RaycastHit _hit, Physics.defaultContactOffset * 3f) ||
                         movable.collider.DoCast(Vector3.down * Physics.defaultContactOffset * 2f, out _hit)) &&
                        PhysicsUtility.IsGroundSurface(_hit.normal))
                    {
                        movable.groundNormal = _hit.normal;
                        movable.force.y = 0f;

                        _isGrounded = true;
                    }
                    else if (movable.isGrounded)
                    {
                        movable.groundNormal.Set(0f, 1f, 0f);
                    }
                }
            }

            // Update ground state.
            movable.SetGroundState(_isGrounded);
        }
        #endregion

        #region Utility
        protected void RegisterCastInfo(RaycastHit _hit)
        {
            buffer.Add(_hit);
        }

        protected void RegisterCastInfos(int _amount)
        {
            DreadfulCollider _collider = movable.collider;
            for (int _i = 0; _i < _amount; _i++)
            {
                buffer.Add(_collider.GetCastHit(_i));
            }
        }
        #endregion
    }

    internal class SimpleCollisionSystem : CollisionSystem
    {
        /// <summary>
        /// Simple collisions, with little less precision on movement.
        /// </summary>
        public SimpleCollisionSystem(Movable _movable) : base(_movable) { }

        #region Collision Calculs
        public override Buffer<RaycastHit> PerformCollisions(Vector3 _velocity)
        {
            buffer.Clear();

            // Calculate collisions recursively.
            CalculateCollisions(_velocity);
            SetGroundState(false);

            // Reduce force according to hit surfaces.
            if (movable.isGrounded)
                movable.force.y = 0f;

            if (!movable.force.IsNull())
            {
                for (int _i = 0; _i < buffer.Count; _i++)
                {
                    RaycastHit _hit = buffer[_i];
                    movable.force = movable.force.ParallelSurface(_hit.normal);
                }
            }

            // Reset instant force and movement after calculs.
            movable.instantForce = movable.movement
                                 = Vector3.zero;

            return buffer;
        }

        private void CalculateCollisions(Vector3 _velocity)
        {
            Rigidbody _rigidbody = movable.rigidbody;
            DreadfulCollider _collider = movable.collider;

            int _amount = _collider.Cast(_velocity, out float _distance);

            // No movement mean object is stuck into something, so return.
            if (_distance == 0f)
                return;

            if (_amount == 0)
            {
                _rigidbody.position += _velocity;
                return;
            }

            // Move rigidbody.
            if ((_distance -= Physics.defaultContactOffset) > 0f)
            {
                _rigidbody.position += _velocity.normalized * _distance;
            }

            RegisterCastInfos(_amount);
        }
        #endregion
    }

    internal class ComplexCollisionSystem : CollisionSystem
    {
        /// <summary>
        /// Complex collisions, with friction and accurate precision.
        /// </summary>
        public ComplexCollisionSystem(Movable _movable) : base(_movable) { }

        #region Collision Calculs
        public override Buffer<RaycastHit> PerformCollisions(Vector3 _velocity)
        {
            buffer.Clear();

            // Calculate collisions recursively.
            CalculateCollisionsRecursively(_velocity);
            SetGroundState(false);

            // Reduce force according to hit surfaces.
            if (movable.isGrounded)
                movable.force.y = 0f;

            if (!movable.force.IsNull())
            {
                for (int _i = 0; _i < buffer.Count; _i++)
                {
                    RaycastHit _hit = buffer[_i];
                    movable.force = movable.force.ParallelSurface(_hit.normal);
                }
            }

            // Reset instant force and movement after calculs.
            movable.instantForce = movable.movement
                                 = Vector3.zero;

            return buffer;
        }

        private void CalculateCollisionsRecursively(Vector3 _velocity, int _recursivityCount = 0)
        {
            Rigidbody _rigidbody = movable.rigidbody;
            DreadfulCollider _collider = movable.collider;

            int _amount = _collider.Cast(_velocity, out float _distance);

            // No movement mean object is stuck into something, so return.
            if (_distance == 0f)
                return;

            if (_amount == 0)
            {
                _rigidbody.position += _velocity;
                return;
            }

            // Move rigidbody and get extra cast velocity.
            if ((_distance -= Physics.defaultContactOffset) > 0f)
            {
                Vector3 _normalizedVelocity = _velocity.normalized;

                _rigidbody.position += _normalizedVelocity * _distance;
                _velocity = _normalizedVelocity * (_velocity.magnitude - _distance);
            }

            RegisterCastInfos(_amount);

            // If reached recursion limit, stop calculs.
            if (_recursivityCount == MaxCollisionCalculRecursivity)
                return;

            // Reduce extra movement according to main impact normals.
            _velocity = _velocity.ParallelSurface(_collider.GetCastHit(0).normal);
            if (!_velocity.IsNull())
            {
                CalculateCollisionsRecursively(_velocity, _recursivityCount + 1);
            }
        }
        #endregion
    }

    internal class CreatureCollisionSystem : CollisionSystem
    {
        /// <summary>
        /// Creature-like collisions, with high precision and slope accuracy.
        /// </summary>
        public CreatureCollisionSystem(Movable _movable) : base(_movable) { }

        #region Collision Calculs
        public override Buffer<RaycastHit> PerformCollisions(Vector3 _velocity)
        {
            Vector3 _normal = movable.groundNormal;
            buffer.Clear();

            // If grounded, adjust velocity according to ground normal.
            if (movable.isGrounded)
                _velocity = ProjectOnNormal(_velocity, movable.groundNormal);

            // Calculate collisions recursively.
            if (CalculateCollisionsRecursively(_velocity, ref _normal))
            {
                movable.groundNormal = _normal;
                SetGroundState(true);
            }
            else
                SetGroundState(false);

            // Reduce force according to hit surfaces.
            if (movable.isGrounded)
                movable.force.y = 0f;

            if (!movable.force.IsNull())
            {
                for (int _i = 0; _i < buffer.Count; _i++)
                {
                    RaycastHit _hit = buffer[_i];
                    movable.force = movable.force.ParallelSurface(_hit.normal);
                }
            }

            // Reset instant force and movement after calculs.
            movable.instantForce = movable.movement
                                 = Vector3.zero;

            return buffer;
        }

        private bool CalculateCollisionsRecursively(Vector3 _velocity, ref Vector3 _normal, int _recursivityCount = 0)
        {
            Rigidbody _rigidbody = movable.rigidbody;
            DreadfulCollider _collider = movable.collider;

            int _amount = _collider.Cast(_velocity, out float _distance);

            // No movement mean object is stuck into something, so return.
            if (_distance == 0f)
                return false;

            if (_amount == 0)
            {
                _rigidbody.position += _velocity;
                GroundSnap(movable, _velocity, _normal);
                return false;
            }

            // Move rigidbody and get extra cast velocity.
            if ((_distance -= Physics.defaultContactOffset) > 0f)
            {
                Vector3 _normalizedVelocity = _velocity.normalized;

                _rigidbody.position += _normalizedVelocity * _distance;
                _velocity = _normalizedVelocity * (_velocity.magnitude - _distance);
            }

            RegisterCastInfos(_amount);

            // If reached recursion limit, stop calculs.
            if (_recursivityCount == MaxCollisionCalculRecursivity)
            {
                GroundSnap(movable, _velocity, _normal);
                return false;
            }

            // Get velocity outside normal surface, as pure value.
            if (_normal.y != 1f)
                _velocity = Quaternion.FromToRotation(_normal, Vector3.up) * _velocity;

            // -------------------------------------- //
            //                                        //
            // -----      Obstacle Dealing      ----- //
            //                                        //
            // -------------------------------------- //

            // Define if obstacle is a ground surface,
            // a slope to climb, a one to slide down on or a real obstacle.
            bool _isGrounded = false;
            _normal = _collider.GetCastHit(0).normal;

            if (PhysicsUtility.IsGroundSurface(_normal))
            {
                _velocity = ProjectOnNormal(_velocity, _normal).ParallelSurface(_normal);
                _isGrounded = true;

                //Debug.LogError("Ground => " + castBuffer[0].collider.name + " | V => " + _velocity.ToStringX(3));
            }
            else if ((_normal.y >= 0f) &&
                    (Mathm.HaveDifferentSignAndNotNull(_velocity.x, _normal.x) ||
                     Mathm.HaveDifferentSignAndNotNull(_velocity.z, _normal.z)))
            {
                // Define if step can be climbed by casting all along it.
                Vector3 _climbVelocity = Vector3.ProjectOnPlane(Vector3.up, _normal).normalized * PhysicsSettings.I.GroundClimbHeight;
                _collider.DoCast(_climbVelocity, out RaycastHit _hit);

                Vector3 _movement = Vector3.zero;
                if ((_hit.distance -= Physics.defaultContactOffset) > 0f)
                {
                    _movement = _climbVelocity.normalized * _hit.distance;
                    _rigidbody.position += _movement;
                }

                // Now, check if step height can be climbed by casting in inverse normal direction.
                // If the step is no more detected, then it can be climbed.
                _climbVelocity = _normal * Physics.defaultContactOffset * -.1f;
                if (_collider.DoCast(_climbVelocity, out RaycastHit _))
                {
                    // When walking against a steep slope,
                    // object need a certain velocity to slide along it.
                    // (Cannot walk on it without a high movement, but be pushed more easily on.)
                    //
                    // If cannot climb or slide, consider obstacle as a 90° wall.
                    float _flatMovement = movable.movement.x + movable.movement.z;
                    float _flatForce = movable.force.x + movable.force.z;

                    if ((_flatMovement < PhysicsSettings.I.SteepSlopeRequiredMovement) &&
                        (_flatForce < PhysicsSettings.I.SteepSlopeRequiredForce))
                    {
                        Vector3 _xz = new Vector3(_velocity.x, 0f, _velocity.z).ParallelSurface(new Vector3(_normal.x, 0f, _normal.z).normalized);
                        _velocity.Set(_xz.x, _velocity.y, _xz.z);

                        _rigidbody.position -= _movement;
                        _normal.Set(0f, 1f, 0f);
                    }
                    else
                    {
                        // Reset rigidbody position,
                        // and transpose velocity on impact normal.
                        _rigidbody.position -= _movement;
                        _velocity = _velocity.ParallelSurface(_normal);

                        _isGrounded = true;
                    }

                    //Debug.LogError("Obstacle => " + castBuffer[0].collider.name + " | V => " + _velocity.ToStringX(3));
                }
                else
                {
                    // If climbable, let rigidbody on top of the step
                    // add opposite climb movement to velocity.
                    _velocity -= new Vector3(Mathf.MoveTowards(_movement.x, 0f, Mathf.Abs(_velocity.x)),
                                             _movement.y,
                                             Mathf.MoveTowards(_movement.z, 0f, Mathf.Abs(_velocity.z)));
                    _normal.Set(0f, 1f, 0f);
                    _isGrounded = true;

                    //Debug.LogError("Climbed => " + castBuffer[0].collider.name);
                }
            }
            else
            {
                // If obstacle is not to climb (Ceiling, down slope),
                // just slide on it.
                _velocity = _velocity.ParallelSurface(_normal);

                //Debug.LogError("Slide On => " + castBuffer[0].collider.name);
            }

            // -------------------------------------- //
            //                                        //
            // -----        End Obstacle        ----- //
            //                                        //
            // -------------------------------------- //

            // If remaining velocity, continue to calculate collisions.
            if (!_velocity.IsNull())
            {
                Vector3 _nextNormal = _normal;
                if (CalculateCollisionsRecursively(_velocity, ref _nextNormal, _recursivityCount + 1))
                {
                    _isGrounded = true;
                    _normal = _nextNormal;
                }
            }

            return _isGrounded;
        }

        // -----------------------

        private Vector3 ProjectOnNormal(Vector3 _velocity, Vector3 _normal)
        {
            // Rotate X & Z velocity to match normal.
            Vector3 _xz = Vector3.ProjectOnPlane(new Vector3(_velocity.x, 0f, _velocity.z), _normal);

            // When going up, do not orientate vertical movement on ground normal
            // to keep a straight trajectory.
            Vector3 _y = ((_velocity.y < 0f) ? _normal
                                             : Vector3.up) * _velocity.y;

            return _xz + _y;
        }

        protected void GroundSnap(Movable _movable, Vector3 _velocity, Vector3 _normal)
        {
            // Get going down velocity.
            _velocity = _normal * Vector3.Dot(_velocity, _normal);

            // If object was grounded and going down, try to snap to ground (slope & steps).
            RaycastHit _hit = default;
            bool _snap = _movable.isGrounded && (_velocity.y <= 0f) &&
                         _movable.collider.DoCast(_normal * -PhysicsSettings.I.GroundSnapHeight, out _hit) &&
                         ((_hit.distance -= Physics.defaultContactOffset) > 0f);

            if (_snap)
            {
                _movable.rigidbody.position += _normal * -_hit.distance;
                RegisterCastInfo(_hit);
            }
        }
        #endregion
    }

    #region Exceptions
    /// <summary>
    /// Exception for invalid <see cref="CollisionSystemType"/>,
    /// when int value is outside enum limits.
    /// </summary>
    public class InvalidCollisionSystemTypeException : Exception
    {
        public InvalidCollisionSystemTypeException() : base() { }

        public InvalidCollisionSystemTypeException(string _message) : base(_message) { }

        public InvalidCollisionSystemTypeException(string _message, Exception _innerException) : base(_message, _innerException) { }
    }
    #endregion
}
