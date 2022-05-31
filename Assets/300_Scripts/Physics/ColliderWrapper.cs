using System;
using UnityEngine;

namespace HorrorPS1.HorrorPhysics
{
    /// <summary>
    /// Internal low-level wrapper for engine 3D primitive colliders.
    /// Used within <see cref="DreadfulCollider"/> for
    /// precise cast and overlap operations.
    /// </summary>
	internal abstract class ColliderWrapper
    {
        #region Wrapper Creator
        /// <summary>
        /// Creates a new appropriated <see cref="ColliderWrapper"/>
        /// for the specified collider.
        /// </summary>
        /// <param name="_collider">Collider to get wrapper for.</param>
        /// <returns>Wrapper configured for specified collider.</returns>
        public static ColliderWrapper CreateWrapper(Collider _collider)
        {
            if (_collider is BoxCollider _box)
            {
                return new BoxColliderWrapper(_box);
            }
            if (_collider is CapsuleCollider _capsule)
            {
                return new CapsuleColliderWrapper(_capsule);
            }
            if (_collider is SphereCollider _sphere)
            {
                return new SphereColliderWrapper(_sphere);
            }

            throw new NonPrimitiveColliderException();
        }
        #endregion

        #region Physics Operations
        /// <summary>
        /// Raycasts from collider using a given velocity.
        /// </summary>
        public abstract bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction);

        /// <summary>
        /// Casts the collider using a given velocity.
        /// </summary>
        public abstract int Cast(Vector3 _direction, RaycastHit[] _buffer, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction);

        /// <summary>
        /// Get overlapping colliders.
        /// </summary>
        public abstract int Overlap(Collider[] _buffer, int _mask, QueryTriggerInteraction _triggerInteraction);
        #endregion

        #region Utility
        /// <summary>
        /// Get world-space non-rotated collider extents.
        /// </summary>
        public abstract Vector3 GetExtents();
        #endregion
    }

    internal class BoxColliderWrapper : ColliderWrapper
    {
        #region Global Members
        public BoxCollider Collider = null;

        // -----------------------

        public BoxColliderWrapper(BoxCollider _collider)
        {
            Collider = _collider;
        }
        #endregion

        #region Physics Operations
        public override bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _offset = Collider.transform.rotation * Vector3.Scale(_direction, GetExtents());
            bool _doHit = Physics.Raycast(Collider.bounds.center + _offset, _direction,
                                          out _hit, _distance, _mask, _triggerInteraction);

            return _doHit;
        }

        public override int Cast(Vector3 _direction, RaycastHit[] _buffer, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _extents = GetExtents() - (Vector3.one * Physics.defaultContactOffset);
            int _amount = Physics.BoxCastNonAlloc(Collider.bounds.center, _extents, _direction,
                                                  _buffer, Collider.transform.rotation, _distance, _mask, _triggerInteraction);

            return _amount;
        }

        public override int Overlap(Collider[] _buffer, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            int _amount = Physics.OverlapBoxNonAlloc(Collider.bounds.center, GetExtents(),
                                                     _buffer, Collider.transform.rotation, _mask, _triggerInteraction);

            return _amount;
        }
        #endregion

        #region Utility
        public override Vector3 GetExtents()
        {
            Vector3 _extents = Collider.transform.TransformVector(Collider.size * .5f);
            return _extents;
        }
        #endregion
    }

    internal class CapsuleColliderWrapper : ColliderWrapper
    {
        #region Global Members
        public CapsuleCollider Collider = null;

        // -----------------------

        public CapsuleColliderWrapper(CapsuleCollider _collider)
        {
            Collider = _collider;
        }
        #endregion

        #region Physics Operations
        public override bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _offset = Collider.transform.rotation * Vector3.Scale(GetExtents(), _direction);
            bool _doHit = Physics.Raycast(Collider.bounds.center + _offset, _direction,
                                          out _hit, _distance, _mask, _triggerInteraction);

            //Debug.LogError("Raycast => " + _doHit + " | " + _distance + " | " + _offset.ToStringX(3) + " | " + Collider.bounds.center.ToStringX(3));

            return _doHit;
        }

        public override int Cast(Vector3 _velocity, RaycastHit[] _buffer, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _offset = GetPointOffset();
            Vector3 _center = Collider.bounds.center;
            float _radius = Collider.radius - Physics.defaultContactOffset;

            int _amount = Physics.CapsuleCastNonAlloc(_center - _offset, _center + _offset, _radius, _velocity,
                                                      _buffer, _distance, _mask, _triggerInteraction);

            return _amount;
        }

        public override int Overlap(Collider[] _buffer, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _offset = GetPointOffset();
            Vector3 _center = Collider.bounds.center;

            int _amount = Physics.OverlapCapsuleNonAlloc(_center - _offset, _center + _offset, Collider.radius,
                                                         _buffer, _mask, _triggerInteraction);

            return _amount;
        }
        #endregion

        #region Utility
        public override Vector3 GetExtents()
        {
            Vector3 _extents;
            switch (Collider.direction)
            {
                // X axis.
                case 0:
                    _extents = new Vector3(Collider.height * .5f, Collider.radius, Collider.radius);
                    break;

                // Y axis.
                case 1:
                    _extents = new Vector3(Collider.radius, Collider.height * .5f, Collider.radius);
                    break;

                // Z axis.
                case 2:
                    _extents = new Vector3(Collider.radius, Collider.radius, Collider.height * .5f);
                    break;

                // This never happen.
                default:
                    throw new InvalidCapsuleHeightException();
            }

            return Collider.transform.TransformVector(_extents);
        }

        public Vector3 GetPointOffset()
        {
            Vector3 _offset;
            switch (Collider.direction)
            {
                // X axis.
                case 0:
                    _offset = new Vector3((Collider.height * .5f) - Collider.radius, 0f, 0f);
                    break;

                // Y axis.
                case 1:
                    _offset = new Vector3(0f, (Collider.height * .5f) - Collider.radius, 0f);
                    break;

                // Z axis.
                case 2:
                    _offset = new Vector3(0f, 0f, (Collider.height * .5f) - Collider.radius);
                    break;

                // This never happen.
                default:
                    throw new InvalidCapsuleHeightException();
            }

            _offset = Collider.transform.TransformVector(_offset);
            return Collider.transform.rotation * _offset;
        }
        #endregion
    }

    internal class SphereColliderWrapper : ColliderWrapper
    {
        #region Global Members
        public SphereCollider Collider = null;

        // -----------------------

        public SphereColliderWrapper(SphereCollider _collider)
        {
            Collider = _collider;
        }
        #endregion

        #region Physics Operations
        public override bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            Vector3 _offset = Collider.transform.rotation * Vector3.Scale(_direction, GetExtents());
            bool _doHit = Physics.Raycast(Collider.bounds.center + _offset, _direction,
                                          out _hit, _distance, _mask, _triggerInteraction);

            return _doHit;
        }

        public override int Cast(Vector3 _velocity, RaycastHit[] _buffer, float _distance, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            float _radius = Collider.radius - Physics.defaultContactOffset;
            int _amount = Physics.SphereCastNonAlloc(Collider.bounds.center, _radius, _velocity,
                                                     _buffer, _distance, _mask, _triggerInteraction);

            return _amount;
        }

        public override int Overlap(Collider[] _buffer, int _mask, QueryTriggerInteraction _triggerInteraction)
        {
            int _amount = Physics.OverlapSphereNonAlloc(Collider.bounds.center, Collider.radius,
                                                        _buffer, _mask, _triggerInteraction);

            return _amount;
        }
        #endregion

        #region Utility
        public override Vector3 GetExtents()
        {
            Vector3 _extents = new Vector3(Collider.radius, Collider.radius, Collider.radius);
            return Collider.transform.TransformVector(_extents);
        }
        #endregion
    }

    #region Exceptions
    /// <summary>
    /// Exception for non primitive collider, forbidding
    /// usage of complex (casts or overlap) physics operations.
    /// </summary>
    public class NonPrimitiveColliderException : Exception
    {
        public NonPrimitiveColliderException() : base() { }

        public NonPrimitiveColliderException(string _message) : base(_message) { }

        public NonPrimitiveColliderException(string _message, Exception _innerException) : base(_message, _innerException) { }
    }

    /// <summary>
    /// Exception for invalid capsule height axis, making
    /// associated collider cast or overlap impossible.
    /// </summary>
    public class InvalidCapsuleHeightException : Exception
    {
        public InvalidCapsuleHeightException() : base() { }

        public InvalidCapsuleHeightException(string _message) : base(_message) { }

        public InvalidCapsuleHeightException(string _message, Exception _innerException) : base(_message, _innerException) { }
    }
    #endregion
}
