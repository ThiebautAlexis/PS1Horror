using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1.HorrorPhysics
{
    /// <summary>
    /// Wrapper for engine 3D primitive colliders.
    /// Use this to perform precise cast and overlap operations.
    /// </summary>
    [Serializable]
    public class DreadfulCollider
    {
        /// <summary>
        /// Maximum distance compared to cast first hit collider
        /// to be considered as a valid hit.
        /// </summary>
        public const float MaxCastDifferenceDetection = .001f;

        /// <summary>
        /// Minimum length distance to be used for collider casts.
        /// </summary>
        public const float MinCastLength = .0001f;

        #region Global Members
        [SerializeField, Enhanced, Required] private Collider collider = null;
        private ColliderWrapper wrapper = null;

        /// <summary>
        /// Default mask used for collision detections.
        /// </summary>
        public int CollisionMask = 0;

        public Collider Collider
        {
            get => collider;
            set
            {
                collider = value;
                Initialize();
            }
        }

        // -----------------------

        /// <summary>
        /// World-space collider bounding box center.
        /// </summary>
        public Vector3 Center => collider.bounds.center;

        /// <summary>
        /// World-space non-rotated collider extents.
        /// </summary>
        public Vector3 Extents => wrapper.GetExtents();
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes this <see cref="DreadfulCollider"/>.
        /// Always call initialization before any use,
        /// preferably on Start or Awake.
        /// </summary>
        public void Initialize()
        {
            int _layer = PhysicsUtility.GetLayerCollisionMask(Collider.gameObject);
            Initialize(_layer);
        }

        /// <summary>
        /// Initializes this <see cref="DreadfulCollider"/>.
        /// Always call initialization before any use,
        /// preferably on Start or Awake.
        /// </summary>
        /// <param name="_collisionMask">Default mask to be used for collider collision detections.</param>
        public void Initialize(int _collisionMask)
        {
            CollisionMask = _collisionMask;
            wrapper = ColliderWrapper.CreateWrapper(Collider);
        }
        #endregion

        #region Bounds
        /// <summary>
        /// Modify the bounds Size according to the Collider Type (Box, Capsule or Sphere)
        /// </summary>
        /// <param name="_boundsSize">New Size of the bounds</param>
        private void ModifyBoundsSize(Vector3 _boundsSize)
        {
            if (collider is BoxCollider _boxCollider)
            {
                _boxCollider.size = _boundsSize;
            }
            else if (collider is CapsuleCollider _capsuleCollider)
            {
                _capsuleCollider.radius = _boundsSize.x;
                _capsuleCollider.height = _boundsSize.y;
            }
            else if (collider is SphereCollider _sphereCollider)
            {
                _sphereCollider.radius = _boundsSize.x;
            }
        }

        /// <summary>
        /// Modify the Center Position of the Bounds according to the Collider Type (Box, Capsule or Sphere)
        /// </summary>
        /// <param name="_boundsCenter">New Center of the bounds</param>
        private void ModifyBoundsCenter(Vector3 _boundsCenter)
        {
            if(collider is BoxCollider _boxCollider)
            {
                _boxCollider.center = _boundsCenter;
            }
            else if(collider is CapsuleCollider _capsuleCollider)
            {
                _capsuleCollider.center = _boundsCenter;
            }
            else if(collider is SphereCollider _sphereCollider)
            {
                _sphereCollider.center = _boundsCenter;
            }
        }

        /// <summary>
        /// Modify the bounds of the Collider and update the dreadful Collider.
        /// </summary>
        /// <param name="_boundsCenter">New Center of the bounds</param>
        /// <param name="_boundsSize">New Size of the bounds</param>
        public void ModifyBounds(Vector3 _boundsCenter, Vector3 _boundsSize)
        {
            ModifyBoundsCenter(_boundsCenter);
            ModifyBoundsSize(_boundsSize);
            //Initialize(CollisionMask);
        }
        
        #endregion

        #region Raycasts
        /// <summary>
        /// Raycasts from this collider using a given velocity.
        /// </summary>
        /// <param name="_velocity">Raycast velocity.</param>
        /// <param name="_hit">Detailed informations on raycast hit.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if raycast hit something on the way, false otherwise.</returns>
        public bool Raycast(Vector3 _velocity, out RaycastHit _hit, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            float _distance = _velocity.magnitude;
            bool _doHit = Raycast(_velocity, out _hit, _distance, CollisionMask, _triggerInteraction);

            return _doHit;
        }

        /// <summary>
        /// Raycasts from this collider using a given velocity.
        /// </summary>
        /// <param name="_velocity">Raycast velocity.</param>
        /// <param name="_hit">Detailed informations on raycast hit.</param>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if raycast hit something on the way, false otherwise.</returns>
        public bool Raycast(Vector3 _velocity, out RaycastHit _hit, int _mask, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            float _distance = _velocity.magnitude;
            bool _doHit = Raycast(_velocity, out _hit, _distance, _mask, _triggerInteraction);

            return _doHit;
        }

        /// <summary>
        /// Raycasts from this collider in a given direction.
        /// </summary>
        /// <param name="_direction">Raycast direction.</param>
        /// <param name="_hit">Detailed informations on raycast hit.</param>
        /// <param name="_distance">Maximum raycast distance.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if raycast hit something on the way, false otherwise.</returns>
        public bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            bool _doHit = Raycast(_direction, out _hit, _distance, CollisionMask, _triggerInteraction);
            return _doHit;
        }

        /// <summary>
        /// Raycasts from this collider in a given direction.
        /// </summary>
        /// <param name="_direction">Cast direction.</param>
        /// <param name="_hit">Detailed informations on raycast hit.</param>
        /// <param name="_distance">Maximum raycast distance.</param>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if raycast hit something on the way, false otherwise.</returns>
        public bool Raycast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask,
                            QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            _direction.Normalize();
            bool _doHit = wrapper.Raycast(_direction, out _hit, _distance, _mask, _triggerInteraction);

            return _doHit;
        }
        #endregion

        #region Casts
        private static readonly RaycastHit[] castBuffer = new RaycastHit[8];

        /// <summary>
        /// Get hit informations from last cast at specified index.
        /// Note that last raycast is from the whole game, not specific
        /// to this collider.
        /// </summary>
        /// <param name="_index">Index to get hit details at.
        /// Should be used in association with informations from Cast methods.</param>
        /// <returns>Details informations about hit at specified index
        /// from last cast.</returns>
        public RaycastHit GetCastHit(int _index)
        {
            return castBuffer[_index];
        }

        // -----------------------

        /// <summary>
        /// Casts this collider using a given velocity.
        /// 
        /// Give first hit distance from object.
        /// </summary>
        /// <param name="_velocity">Velocity used for cast.</param>
        /// <param name="_distance">Traveled distance before first collision.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if collided with something on the way, false otherwise.</returns>
        public bool DoCast(Vector3 _velocity, out float _distance, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_velocity, out RaycastHit _hit, _velocity.magnitude, CollisionMask, _triggerInteraction);

            _distance = _hit.distance;
            return _amount > 0;
        }

        /// <summary>
        /// Casts this collider using a given velocity.
        /// 
        /// Give detailed informations about main trajectory collision.
        /// </summary>
        /// <param name="_velocity">Velocity used for cast.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if collided with something on the way, false otherwise.</returns>
        public bool DoCast(Vector3 _velocity, out RaycastHit _hit, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_velocity, out _hit, _velocity.magnitude, CollisionMask, _triggerInteraction);
            return _amount > 0;
        }

        /// <summary>
        /// Casts this collider using a given velocity.
        /// 
        /// Give detailed informations about main trajectory collision.
        /// </summary>
        /// <param name="_velocity">Velocity used for cast.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if collided with something on the way, false otherwise.</returns>
        public bool DoCast(Vector3 _velocity, out RaycastHit _hit, int _mask, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_velocity, out _hit, _velocity.magnitude, _mask, _triggerInteraction);
            return _amount > 0;
        }

        /// <summary>
        /// Casts this collider in a given direction.
        /// 
        /// Give detailed informations about main trajectory collision.
        /// </summary>
        /// <param name="_direction">Cast direction.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_distance">Maximum cast distance.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if collided with something on the way, false otherwise.</returns>
        public bool DoCast(Vector3 _direction, out RaycastHit _hit, float _distance, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_direction, out _hit, _distance, CollisionMask, _triggerInteraction);
            return _amount > 0;
        }

        /// <summary>
        /// Casts this collider in a given direction.
        /// 
        /// Give detailed informations about main trajectory collision.
        /// </summary>
        /// <param name="_direction">Cast direction.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_distance">Maximum cast distance.</param>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>True if collided with something on the way, false otherwise.</returns>
        public bool DoCast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask,
                           QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_direction, out _hit, _distance, _mask, _triggerInteraction);
            return _amount > 0;
        }

        /// <summary>
        /// Casts this collider using a given velocity.
        /// 
        /// Indicates trajectory consistent hits amount,
        /// and first hit distance from object.
        /// </summary>
        /// <param name="_velocity">Velocity used for cast.</param>
        /// <param name="_distance">Traveled distance before first collision.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>Total trajectory consistent hit amount.</returns>
        public int Cast(Vector3 _velocity, out float _distance, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_velocity, out RaycastHit _hit, _velocity.magnitude, CollisionMask, _triggerInteraction);

            _distance = _hit.distance;
            return _amount;
        }

        /// <summary>
        /// Casts this collider using a given velocity.
        /// 
        /// Indicates trajectory consistent hits amount,
        /// and give detailed informations about the main one.
        /// </summary>
        /// <param name="_velocity">Velocity used for cast.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>Total trajectory consistent hit amount.</returns>
        public int Cast(Vector3 _velocity, out RaycastHit _hit, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            float _distance = _velocity.magnitude;
            int _amount = Cast(_velocity, out _hit, _distance, CollisionMask, _triggerInteraction);

            return _amount;
        }

        /// <summary>
        /// Casts this collider in a given direction.
        /// 
        /// Indicates trajectory consistent hits amount,
        /// and give detailed informations about the main one.
        /// </summary>
        /// <param name="_direction">Cast direction.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_distance">Maximum cast distance.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>Total trajectory consistent hit amount.</returns>
        public int Cast(Vector3 _direction, out RaycastHit _hit, float _distance, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int _amount = Cast(_direction, out _hit, _distance, CollisionMask, _triggerInteraction);
            return _amount;
        }

        /// <summary>
        /// Casts this collider in a given direction.
        /// 
        /// Indicates trajectory consistent hits amount,
        /// and give detailed informations about the main one.
        /// </summary>
        /// <param name="_direction">Cast direction.</param>
        /// <param name="_hit">Main trajectory hit detailed informations.</param>
        /// <param name="_distance">Maximum cast distance.</param>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the cast interact with triggers?</param>
        /// <returns>Total trajectory consistent hit amount.</returns>
        public int Cast(Vector3 _direction, out RaycastHit _hit, float _distance, int _mask,
                        QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            _direction.Normalize();
            _distance += Physics.defaultContactOffset * 2;
            int _amount = wrapper.Cast(_direction, castBuffer, _distance, _mask, _triggerInteraction);
            if (_amount > 0)
            {
                // Remove this object collider if detected.
                if (castBuffer[_amount - 1].collider == collider)
                {
                    _amount--;
                    if (_amount == 0)
                    {
                        _hit = GetDefaultHit();
                        return 0;
                    }
                }

                #if DEBUG_LOGS
                // Debug thing. Should be remove one day.
                for (int _i = 0; _i < _amount; _i++)
                {
                    if (castBuffer[_i].collider == collider)
                        collider.LogError($"Found Collider => {_i}/{_amount}");
                }
                #endif

                PhysicsUtility.SortRaycastHitByDistance(castBuffer, _amount);

                _hit = castBuffer[0];
                _hit.distance = Mathf.Max(0, _hit.distance - Physics.defaultContactOffset);

                for (int _i = 1; _i < _amount; _i++)
                {
                    if (castBuffer[_i].distance > (_hit.distance + MaxCastDifferenceDetection))
                        return _i;
                }
            }
            else
                _hit = GetDefaultHit();

            return _amount;

            // ----- Local Method ----- //

            RaycastHit GetDefaultHit()
            {
                RaycastHit _hit = new RaycastHit();
                _hit.distance = _distance - Physics.defaultContactOffset;

                return _hit;
            }
        }
        #endregion

        #region Overlaps
        private static readonly Collider[] overlapBuffer = new Collider[8];

        /// <summary>
        /// Get collider at specified index from last overlap.
        /// Note that last overlap is from the whole game, not specific
        /// to this collider.
        /// </summary>
        /// <param name="_index">Index to get collider at.
        /// Should be used in association with amount from Overlap methods.</param>
        /// <returns>Colliders at specified index from last overlap.</returns>
        public Collider GetOverlapCollider(int _index)
        {
            return overlapBuffer[_index];
        }

        // -----------------------

        /// <summary>
        /// Get informations about overlapping colliders.
        /// </summary>
        /// <param name="_triggerInteraction">How should the overlap interact with triggers?</param>
        /// <returns>Amount of overlapping colliders.</returns>
        public int Overlap(QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Collide)
        {
            int _amount = wrapper.Overlap(overlapBuffer, CollisionMask, _triggerInteraction);
            return _amount;
        }

        /// <summary>
        /// Get informations about overlapping colliders.
        /// </summary>
        /// <param name="_mask"><see cref="LayerMask"/> to use for collision detection.</param>
        /// <param name="_triggerInteraction">How should the overlap interact with triggers?</param>
        /// <returns>Amount of overlapping colliders.</returns>
        public int Overlap(int _mask, QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Collide)
        {
            int _amount = wrapper.Overlap(overlapBuffer, _mask, _triggerInteraction);
            return _amount;
        }

        // -----------------------

        /// <summary>
        /// Sorts overlapping colliders.
        /// </summary>
        /// <param name="_comparison">Comparison to use to sort array.</param>
        public void SortOverlaps(Comparison<Collider> _comparison)
        {
            Array.Sort(overlapBuffer, _comparison);
        }
        #endregion
    }
}
