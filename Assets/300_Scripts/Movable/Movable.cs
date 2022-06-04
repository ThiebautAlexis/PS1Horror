using EnhancedEditor;
using HorrorPS1.Core;
using HorrorPS1.HorrorPhysics;
using HorrorPS1.Settings;
using HorrorPS1.Tools;
using UnityEngine;
using HorrorPS1;

namespace HorrorPS1.Movable
{
    /// <summary>
    /// Controller contract for a <see cref="Movable"/>.
    /// Use this to receive callbacks and override this object default behaviour.
    /// <para/>
    /// Should be set on Awake, as some operations are made on the Start callback.
    /// </summary>
    public interface IMovableController
    {
        #region Collision Configuration
        /// <summary>
        /// Determines how this object react to collisions and move in space.
        /// </summary>
        CollisionSystemType CollisionType { get; }

        /// <summary>
        /// <see cref="LayerMask"/> used for this object collision detections.
        /// </summary>
        int CollisionMask { get; }
        #endregion

        #region Animation
        /// <summary>
        /// Called when setting root-motion-related
        /// horizontal obstacle value for animation.
        /// </summary>
        /// <param name="_hash">Animator parameter identifier, as hash.</param>
        /// <param name="_value">Parameter value.</param>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnPlayHorizontalObstacle(int _hash, int _value);

        /// <summary>
        /// Called when setting root-motion-related
        /// vertical obstacle value for animation.
        /// </summary>
        /// <param name="_hash">Animator parameter identifier, as hash.</param>
        /// <param name="_value">Parameter value.</param>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnPlayVerticallObstacle(int _hash, int _value);
        #endregion

        #region Movable Behaviour
        /// <summary>
        /// Called when applying gravity on this object.
        /// </summary>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnApplyGravity();

        /// <summary>
        /// Called when computing this object velocity.
        /// </summary>
        /// <param name="_force">Force velocity.</param>
        /// <param name="_instantForce">Instant force velocity (not affected by DeltaTime).</param>
        /// <param name="_movement">Movement velocity.</param>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnComputeVelocity(ref Vector3 _force, ref Vector3 _instantForce, ref Vector3 _movement);
        #endregion

        #region Callbacks
        /// <summary>
        /// Called when this object gets up after being kicked up.
        /// <para/>
        /// You can use this to set related animation and restart your behaviour.
        /// </summary>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnGetUp();

        /// <summary>
        /// Called after this object velocity has been applied.
        /// </summary>
        /// <param name="_velocity">Object initial velocity for this frame.</param>
        /// <param name="_displacement">Real object displacement during this frame.</param>
        /// <returns>True to completely override this behaviour, false otherwise.</returns>
        bool OnAppliedVelocity(Vector3 _velocity, Vector3 _displacement);

        /// <summary>
        /// Called after this object ground state has changed.
        /// </summary>
        /// <param name="_isGrounded">Is the object grounded?</param>
        /// <returns>True to completely override this behaviour, false otherwise</returns>
        bool OnSetGrounded(bool _isGrounded);
        #endregion
    }

    /// <summary>
    /// To be used on every moving object of the game.
    /// <para/>
    /// Provides multiple common utilities to properly move
    /// objects in space.
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Object rigidbody.
        /// </summary>
        Rigidbody Rigidbody { get; }

        // -----------------------

        /// <summary>
        /// Set this object position.
        /// Use this instead of setting <see cref="Transform.position"/>.
        /// </summary>
        void SetPosition(Vector3 _position);

        /// <summary>
        /// Set this object rotation.
        /// Use this instead of setting <see cref="Transform.rotation"/>.
        /// </summary>
        void SetRotation(Quaternion _rotation);
    }

    /// <summary>
    /// Base class for every moving object of the game with complex velocity.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Movable : HorrorBehaviour, IMovable, IMovableUpdate
    {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Movable;

        #region Collision Settings
        /// <summary>
        /// Type of collision system used to calculate
        /// how this object move in space.
        /// </summary>
        public virtual CollisionSystemType CollisionType
        {
            get
            {
                if (useController)
                    return controller.CollisionType;

                return CollisionSystemType.Complex;
            }
        }

        /// <summary>
        /// <see cref="LayerMask"/> to be used for object collision detections.
        /// Uses GameObject layer collision mask from <see cref="Physics"/> settings by default.
        /// </summary>
        public virtual int CollisionMask
        {
            get
            {
                if (useController)
                    return controller.CollisionMask;

                return PhysicsUtility.GetLayerCollisionMask(gameObject);
            }
        }
        #endregion

        #region Global Members
        [Section("Movable")]

        [SerializeField, Enhanced, Required] internal protected new Rigidbody rigidbody = null;
        [SerializeField, Enhanced, Inline] internal protected new DreadfulCollider collider = new DreadfulCollider();

        [SerializeField, Enhanced, Required] protected MovableAttributes attributes = null;
        [SerializeField, Enhanced, Required] protected Transform rootMotion = null;

        [HorizontalLine(SuperColor.Green)]

        public bool UseGravity = true;
        
        [SerializeField, ReadOnly] protected int facingSide = 1;
        [SerializeField, ReadOnly] protected bool usingRootMotion = false;
        [SerializeField, ReadOnly] internal protected bool isGrounded = false;        

        [Space(5f)]

        [SerializeField, ReadOnly] protected float speed = 1f;
        [SerializeField, ReadOnly] protected float velocityCoef = 1f;

        [Space(5f)]

        [SerializeField, ReadOnly] internal protected Vector3 groundNormal = Vector3.up;

        // -----------------------

        public Rigidbody Rigidbody => rigidbody;

        public bool IsGrounded => isGrounded;

        public float Speed => speed;
        public float VelocityCoef => velocityCoef;

        // -----------------------

        private bool useController = false;
        private IMovableController controller = null;

        /// <summary>
        /// Use this to setup this <see cref="Movable"/> controller.
        /// </summary>
        public IMovableController Controller
        {
            get => controller;
            set
            {
                useController = value != null;
                controller = value;
            }
        }

        // -----------------------

        private CollisionSystem collisionSystem = null;

        /// <summary>
        /// Default empty buffer when no collision has been performed.
        /// </summary>
        protected static Buffer<RaycastHit> defaultBuffer => CollisionSystem.DefaultBuffer;

        // -----------------------
        //
        // Velocity variables.
        //
        // Movable class velocity is composed of 3 Vector3:
        //  • Force, which is related to external forces, having an impact in duration (like wind);
        //  • Instant Force, also external forces but applied for one frame only (like recoil);
        //  • Movement, the velocity applied by the object itself (like walking).

        [HorizontalLine]

        [SerializeField] internal protected Vector3 force =           Vector3.zero;
        [SerializeField] internal protected Vector3 instantForce =    Vector3.zero;
        [SerializeField] internal protected Vector3 movement =        Vector3.zero;

        /// <summary>
        /// This object external force velocity (decreased over time).
        /// </summary>
        public Vector3 Force => force;

        /// <summary>
        /// This object external instant force velocity (for this frame only).
        /// </summary>
        public Vector3 InstantForce => instantForce;

        /// <summary>
        /// This object own velocity (this frame only).
        /// </summary>
        public Vector3 Movement => movement;

        // -----------------------

        /// <summary>
        /// This object velocity for this frame
        /// (with deltaTime and speed coefficient applied).
        /// </summary>
        public Vector3 Velocity
        {
            get            {
                Vector3 _movement = new Vector3(movement.x * speed, movement.y, movement.z * speed);
                Vector3 _velocity = (((force + _movement) * Time.deltaTime) + instantForce) * velocityCoef;

                return _velocity;
            }
        }

        /// <summary>
        /// This object raw velocity,
        /// that is the simple addition of its force, instant force and movement.
        /// </summary>
        public Vector3 RawVelocity
        {
            get
            {
                Vector3 _velocity = force + movement + instantForce;
                return _velocity;
            }
        }

        /// <summary>
        /// This object raw flat velocity,
        /// that is <see cref="RawVelocity"/> only on the X and Z axises.
        /// </summary>
        public Vector2 RawFlatVelocity
        {
            get
            {
                Vector2 _velocity = new Vector2(force.x + instantForce.x + movement.x,
                                                force.z + instantForce.z + movement.z);

                return _velocity;
            }
        }

        /// <summary>
        /// This object velocity affected by speed for this frame
        /// (with all coefficients applied).
        /// </summary>
        public Vector3 SpeedVelocity
        {
            get
            {
                Vector3 _velocity = new Vector3(movement.x, 0f, movement.z);
                _velocity *= speed * Time.deltaTime * velocityCoef;

                return _velocity;
            }
        }

        public float MaxSpeed => attributes._speedRange.y;
        #endregion

        #region Animation
        public static readonly int HorizontalObstacle_Hash = Animator.StringToHash("HorizontalObstacle");
        public static readonly int VerticalObstacle_Hash = Animator.StringToHash("VerticalObstacle");

        private int horizontalObstacle = 0;
        private int verticalObstacle = 0;

        // -----------------------

        protected virtual bool PlayHorizontalObstacle(int _value)
        {
            if (horizontalObstacle == _value)
                return true;

            // Only update value when different.
            horizontalObstacle = _value;

            bool _isOverride = useController && controller.OnPlayHorizontalObstacle(HorizontalObstacle_Hash, _value);
            return _isOverride;
        }

        protected virtual bool PlayVerticalObstacle(int _value)
        {
            if (verticalObstacle == _value)
                return true;

            // Only update value when different.
            verticalObstacle = _value;

            bool _isOverride = useController && controller.OnPlayVerticallObstacle(VerticalObstacle_Hash, _value);
            return _isOverride;
        }
        #endregion

        #region State Callbacks
        protected override void OnDeactivation()
        {
            base.OnDeactivation();

            ExitTriggers();
        }
        #endregion

        #region Facing Side
        /// <summary>
        /// Flips the object (make it face opposite side).
        /// </summary>
        public void Flip()
        {
            Vector3 _scale = transform.localScale;
            _scale.x *= -1f;

            facingSide *= -1;
            transform.localScale = _scale;
        }

        /// <summary>
        /// Makes the object face a certain side.
        /// </summary>
        /// <param name="_facingSide">Side to look (1 for right, -1 for left).</param>
        public void Flip(int _facingSide)
        {
            if (facingSide != _facingSide)
            {
                Flip();
            }
        }
        #endregion

        #region Velocity Coefficients
        private readonly Buffer<float> velocityCoefficients = new Buffer<float>(1);

        // -----------------------

        /// <summary>
        /// Adds a coefficient to this object velocity.
        /// </summary>
        public void AddVelocityCoef(float _coef)
        {
            if (_coef != 0f)
            {
                velocityCoefficients.Add(_coef);
                velocityCoef *= _coef;
            }
            else
            {
                this.LogWarning("You are trying to add a null velocity coefficient. This is not allowed");
            }
        }

        /// <summary>
        /// Removes a coefficient from this object velocity.
        /// </summary>
        public void RemoveVelocityCoef(float _coef)
        {
            if ((_coef != 0f) && velocityCoefficients.Contains(_coef))
            {
                velocityCoefficients.Remove(_coef);
                velocityCoef /= _coef;
            }
            else
            {
                this.LogWarning("You are trying to remove an invalid velocity coefficient. This is not allowed");
            }
        }

        /// <summary>
        /// Resets all this object velocity coefficients.
        /// </summary>
        public void ResetVelocityCoef()
        {
            velocityCoef = 1f;
            velocityCoefficients.Clear();
        }
        #endregion

        #region Velocity Modifiers
        /// <summary>
        /// Adds a force to this object velocity
        /// (force is decreased over time).
        /// </summary>
        public void AddForce(Vector3 _force)
        {
            force += _force;
        }

        /// <summary>
        /// Adds an instant force to this object velocity
        /// (instant force is for this frame only).
        /// </summary>
        public void AddInstantForce(Vector3 _instantForce)
        {
            instantForce += _instantForce;
        }

        /// <summary>
        /// Adds a movement to this object velocity
        /// (movement is this object own velocity, for this frame only).
        /// </summary>
        public void AddMovement(Vector3 _movement, bool _autoFlip = true)
        {
            AddHorizontalMovement(_movement.x, _autoFlip);
            AddVerticalMovement(_movement.y);
            AddVerticalMovement(_movement.z);
        }

        // -----------------------

        /// <summary>
        /// Adds an horizontal movement to this object velocity, affected by speed
        /// (movement is this object own velocity, for this frame only).
        /// </summary>
        public void AddHorizontalMovement(float _movement, bool _autoFlip = true)
        {
            if (_movement == 0f)
                return;

            // Flips this object if not facing movement direction.
            if (_autoFlip && (Mathm.Sign(_movement) != facingSide))
            {
                Flip();
            }

            movement.x += _movement;
        }

        /// <summary>
        /// Adds a vertical movement to this object velocity, not affected by speed
        /// (movement is this object own velocity, for this frame only).
        /// </summary>
        public void AddVerticalMovement(float _movement)
        {
            movement.y += _movement;
        }

        /// <summary>
        /// Adds a forward movement to this object velocity, affected by speed
        /// (movement is this object own velocity, for this frame only).
        /// </summary>
        public void AddForwardMovement(float _movement)
        {
            movement.z += _movement;
        }
        #endregion

        #region Velocity Utility
        private float speedCurveVar = 0f;

        // -----------------------

        /// <summary>
        /// Updates this object speed.
        /// </summary>
        private void UpdateSpeed()
        {
            if (!Mathm.AreEqual(movement.x, movement.z, 0f))
            {
                speedCurveVar += Time.deltaTime;
                speed = attributes.EvaluateSpeed(speedCurveVar);
            }
            else
            {
                ResetSpeed();
            }
        }

        /// <summary>
        /// Resets this object speed.
        /// </summary>
        public virtual void ResetSpeed()
        {
            speed = speedCurveVar
                  = 0f;
        }

        /// <summary>
        /// Completely resets this object velocity
        /// by settings its force, instant force and movement to zero.
        /// </summary>
        public virtual void ResetVelocity()
        {
            ResetSpeed();
            movement = force
                     = instantForce
                     = Vector3.zero;
        }
        #endregion

        #region Velocity Compute
        private Vector2 previousFlatForce = new Vector2();
        private Vector2 previousFlatVelocity = new Vector2();

        // -----------------------

        /// <summary>
        /// Computes the object velocity just before its collision calculs.
        /// </summary>
        protected virtual bool ComputeVelocity()
        {
            // Controller callback.
            if (useController && controller.OnComputeVelocity(ref force, ref instantForce, ref movement))
                return true;

            // Slowly decrease force over time.
            if (force.x != 0f)
            {
                ComputeVelocityAxis(ref force.x, ref movement.x, instantForce.x, previousFlatForce.x, previousFlatVelocity.x);
            }

            if (force.z != 0f)
            {
                ComputeVelocityAxis(ref force.z, ref movement.z, instantForce.z, previousFlatForce.y, previousFlatVelocity.y);
            }

            previousFlatForce.Set(force.x, force.z);
            previousFlatVelocity = RawFlatVelocity;

            // If going to opposite force direction, accordingly reduce force and movement.
            if (Mathm.HaveDifferentSignAndNotNull(force.y, movement.y))
            {
                float _forceDeceleration = Mathf.Abs(movement.y);

                movement.y = Mathf.MoveTowards(movement.y, 0f, Mathf.Abs(force.y));
                force.y = Mathf.MoveTowards(force.y, 0f, _forceDeceleration * Time.deltaTime);
            }

            return false;
        }

        private void ComputeVelocityAxis(ref float _force, ref float _movement, float _instantForce, float _previousForce, float _previousVelocity)
        {
            float _forceDeceleration = isGrounded
                                       ? PhysicsSettings.I.GroundDecelerationForce
                                       : PhysicsSettings.I.AirDecelerationForce;

            // When going opposite force direction, reduce both force and movement accordingly.
            // When going the same direction, reduce movement to its extra velocity compared to force.
            if (_movement != 0f)
            {
                float _movementDeceleration = Mathf.Abs(_force);
                if (Mathm.HaveDifferentSign(_force, _movement))
                {
                    _movementDeceleration *= Time.deltaTime;
                    _forceDeceleration = Mathf.Max(_forceDeceleration, Mathf.Abs(_movement) * 2f);
                }

                _movement = Mathf.MoveTowards(_movement, 0f, _movementDeceleration);
            }

            // When a velocity is added to opposite force direction, both are reduce accordingly.
            //
            // But when this opposite instant velocity is suddenly stopping, we need to reduce
            // the force to get a smooth transition, otherwise the object will suddenly resume
            // int its strong force velocity.
            float _previousInstantXVelocity = _previousVelocity - _previousForce;

            if (Mathm.HaveDifferentSignAndNotNull(_previousInstantXVelocity, _previousForce))
            {
                float _instanceVelocity = _instantForce + _movement;
                float _difference = Mathf.Abs(_previousInstantXVelocity);

                if (!Mathm.HaveDifferentSign(_previousInstantXVelocity, _instanceVelocity))
                {
                    _difference -= Mathf.Abs(_instanceVelocity);
                }

                if (_difference > 0f)
                {
                    _force = Mathf.MoveTowards(_force, 0f, _difference);
                }
            }

            _force = Mathf.MoveTowards(_force, 0f, _forceDeceleration * Time.deltaTime);
        }
        #endregion

        #region Root Motion
        private bool isApplyingMotionForce = false;
        private Vector3 rootMotionPosition = new Vector3();
        private Vector3 rootMotionVelocity = new Vector3();

        // -----------------------

        private void ApplyRootMotion()
        {
            if (isApplyingMotionForce)
            {
                isApplyingMotionForce = false;
            }
            else
            {
                Vector3 _velocity = rootMotion.localPosition - rootMotionPosition;
                _velocity.x *= facingSide;

                AddInstantForce(_velocity);

                rootMotionVelocity = _velocity;
                rootMotionPosition = rootMotion.localPosition;
            }
        }

        // -----------------------

        /// <summary>
        /// Starts this object root motion,
        /// used to add velocity from animation.
        /// </summary>
        public void StartRootMotion()
        {
            if (!usingRootMotion)
            {
                usingRootMotion = true;
                isApplyingMotionForce = false;

                PlayHorizontalObstacle(0);
                PlayVerticalObstacle(isGrounded ? 1 : 0);
            }

            rootMotionPosition.Set(0f, 0f, 0f);
        }

        /// <summary>
        /// Stops this object root motion (object velocity from animation),
        /// after this frame.
        /// </summary>
        public void StopRootMotion()
        {
            Vector3 _velocity = rootMotion.localPosition - rootMotionPosition;
            _velocity.x *= facingSide;

            AddInstantForce(_velocity);
            StopRootMotionImmediatly();
        }

        /// <summary>
        /// Immediatly stops this object root motion,
        /// instead of <see cref="StopRootMotion"/> which is keeping this frame velocity.
        /// </summary>
        public void StopRootMotionImmediatly()
        {
            usingRootMotion = false;
        }

        // -----------------------

        /// <summary>
        /// Apply force on this object (decreased over time)
        /// based on last root motion velocity.
        /// </summary>
        public void ApplyRootMotionForce()
        {
            isApplyingMotionForce = true;

            Vector3 _force = rootMotionVelocity / Time.deltaTime;
            if ((_force.y < 0f) && (_force.x != 0f) && isGrounded)
            {
                _force.x *= PhysicsSettings.I.OnGroundedForceMultiplier;
            }

            AddForce(_force);
            rootMotionPosition = rootMotion.localPosition;
        }

        /// <summary>
        /// Resets this object root-motion-related memory values
        /// to reset its root motion transform position to 0.
        /// </summary>
        public void ResetRootMotion()
        {
            rootMotionPosition.Set(0f, 0f, 0f);
            rootMotionVelocity.Set(0f, 0f, 0f);
        }
        #endregion

        #region Gravity
        /// <summary>
        /// Applies gravity on this object.
        /// Override this to specify custom gravity.
        /// <para/>
        /// Use <see cref="AddGravity"/> and <see cref="AddGravity(float, float)"/>
        /// for quick implementation.
        /// </summary>
        protected virtual bool ApplyGravity()
        {
            // Do not apply gravity while using root motion vertically.
            if (usingRootMotion && (rootMotionPosition.y != 0f))
                return true;

            // Controller callback.
            if (useController && controller.OnApplyGravity())
                return true;

            AddGravity();
            return false;
        }

        // -----------------------

        /// <summary>
        /// Adds gravity as force on this object.
        /// Uses game standard gravity.
        /// </summary>
        public void AddGravity()
        {
            if (force.y > PhysicsSettings.I.MaxGravity)
            {
                float _gravity = Mathf.Max(Physics.gravity.y * Time.deltaTime,
                                           PhysicsSettings.I.MaxGravity - force.y);

                AddForce(new Vector3(0f, _gravity, 0f));
            }
        }

        /// <summary>
        /// Adds gravity as force on this object.
        /// Uses coefficients applied on game standard gravity.
        /// </summary>
        /// <param name="_gravityCoef">Coefficient applied to standard gravity.</param>
        /// <param name="_maxGravityCoef">Coefficient applied to maximum allowed gravity value.</param>
        public void AddGravity(float _gravityCoef, float _maxGravityCoef)
        {
            float _maxGravityValue = PhysicsSettings.I.MaxGravity * _maxGravityCoef;
            if (force.y > _maxGravityValue)
            {
                float _gravity = Mathf.Max(Physics.gravity.y * _gravityCoef * Time.deltaTime,
                                           _maxGravityValue - force.y);

                AddForce(new Vector3(0f, _gravity, 0f));
            }
        }
        #endregion

        #region Parenting
        protected bool isParented = false;
        private Transform parent = null;        

        [SerializeField, ReadOnly] private Vector3 previousParentPosition = new Vector3();
        [SerializeField, ReadOnly] private Quaternion previousParentRotation = Quaternion.identity;

        // -----------------------

        private void FollowParent()
        {
            Vector3 _parentPos = parent.position;
            Quaternion _parentRot = parent.rotation;

            Vector3 _positionDifference = _parentPos - previousParentPosition;
            previousParentPosition = _parentPos;

            Quaternion _rotationDifference = _parentRot * Quaternion.Inverse(previousParentRotation);
            previousParentRotation = _parentRot;

            Vector3 _newPosition = rigidbody.position + _positionDifference;
            Vector3 _difference = _newPosition - _parentPos;

            _newPosition = _parentPos + (_rotationDifference * _difference);
            Quaternion _newRotation = transform.rotation * _rotationDifference;
            
            SetPositionAndRotation(_newPosition, _newRotation);
        }

        // -----------------------

        /// <summary>
        /// Parents this movable to a specific <see cref="Transform"/>.
        /// </summary>
        public void Parent(Transform _parent)
        {
            parent = _parent;
            isParented = true;

            previousParentPosition = _parent.position;
            previousParentRotation = _parent.rotation;
        }

        /// <summary>
        /// Unparents this object from any <see cref="Transform"/>.
        /// </summary>
        public void Unparent()
        {
            parent = null;
            isParented = false;
        }
        #endregion

        #region Transform Utility
        /// <summary>
        /// Sets this object position.
        /// Use this instead of setting <see cref="Transform.position"/>.
        /// </summary>
        public void SetPosition(Vector3 _position)
        {
            rigidbody.position = _position;
            shouldBeRefreshed = true;
        }

        /// <summary>
        /// Sets this object rotation.
        /// Use this instead of setting <see cref="Transform.rotation"/>.
        /// </summary>
        public void SetRotation(Quaternion _rotation)
        {
            rigidbody.rotation = _rotation;
            transform.rotation = _rotation;

            shouldBeRefreshed = true;
        }

        /// <summary>
        /// Sets this object position and rotation.
        /// Use this instead of setting <see cref="Transform.position"/> and <see cref="Transform.rotation"/>.
        /// </summary>
        public void SetPositionAndRotation(Vector3 _position, Quaternion _rotation)
        {
            SetPosition(_position);
            SetRotation(_rotation);
        }
        #endregion

        #region Core Routine
        private bool shouldBeRefreshed = false;

        // -----------------------

        #pragma warning disable UNT0006
        void IMovableUpdate.Update() => MovableUpdate();

        /// <summary>
        /// Updates this object position according to its velocity.
        /// Called every frame while the object is enable.
        /// </summary>
        protected virtual void MovableUpdate()
        {
            if (GameStatesManager.currentGameState != GameStatesManager.InGameState)
                return;

            // Stand object by its parent.
            if (isParented)
                FollowParent();

            // Refresh position before collision calculs, if needed.
            if (shouldBeRefreshed)
            {
                RefreshOverlaps();
                shouldBeRefreshed = false;
            }

            // Move object according to animation root motion.
            if (usingRootMotion)
                ApplyRootMotion();

            // Apply gravity to this object.
            if (UseGravity)
                ApplyGravity();

            // Apply velocity and move the object.
            Vector3 _rawVelocity = RawVelocity;
            if (!_rawVelocity.IsNull())
            {
                ComputeVelocity();
                collisionSystem.ComputeVelocity();
                UpdateSpeed();

                Vector3 _velocity = Velocity;
                Vector3 _lastPosition = rigidbody.position;

                Buffer<RaycastHit> _buffer = collisionSystem.PerformCollisions(_velocity);
                UpdatePosition();

                Vector3 _displacement = rigidbody.position - _lastPosition;
                OnAppliedVelocity(_velocity, _displacement, _buffer);
            }
            else
                OnAppliedVelocity(_rawVelocity, _rawVelocity, CollisionSystem.DefaultBuffer);
        }

        // -----------------------

        internal void SetGroundState(bool _isGrounded)
        {
            if (isGrounded != _isGrounded)
            {
                isGrounded = _isGrounded;
                OnSetGrounded(_isGrounded);
            }
        }

        protected void UpdatePosition()
        {
            // Manage overlapping colliders.
            RefreshOverlaps();

            // Update object position and its related properties.
            Vector3 _position = rigidbody.position;
            transform.position = _position;
        }
        #endregion

        #region Collision Overlaps
        private static readonly Buffer<Trigger> triggerBuffer = new Buffer<Trigger>(3);
        private readonly Buffer<Trigger> triggerOverlaps = new Buffer<Trigger>(2);

        // -----------------------

        private void RefreshOverlaps()
        {
            // Get all overlapping colliders, then extract from physics ones and manage triggers behaviour.
            int _overlapAmount = collider.Overlap();
            triggerBuffer.Clear();

            for (int _i = 0; _i < _overlapAmount; _i++)
            {
                Collider _collider = collider.GetOverlapCollider(_i);

                if (_collider.isTrigger)
                {
                    // Manage trigger interactions.
                    if (_collider.TryGetComponent(out Trigger _trigger))
                    {
                        triggerBuffer.Add(_trigger);

                        if (HasEnteredTrigger(_trigger))
                        {
                            _trigger.OnEnter(this);
                            triggerOverlaps.Add(_trigger);
                        }
                    }
                }
                else if ((_collider != collider.Collider) &&
                         Physics.ComputePenetration(collider.Collider, rigidbody.position, transform.rotation,
                                                    _collider, _collider.transform.position, _collider.transform.rotation,
                                                    out Vector3 _direction, out float _distance))
                {
                    rigidbody.position += _direction * _distance;
                }
            }

            // Exit from no more overlapping triggers.
            for (int _i = triggerOverlaps.Count; _i-- > 0;)
            {
                Trigger _trigger = triggerOverlaps[_i];

                if (HasExitedTrigger(_trigger))
                {
                    _trigger.OnExit(this);
                    triggerOverlaps.RemoveAt(_i);
                }
            }

            // ----- Local Methods ----- //

            bool HasEnteredTrigger(Trigger _trigger)
            {
                for (int _i = 0; _i < triggerOverlaps.Count; _i++)
                {
                    Trigger _other = triggerOverlaps[_i];
                    if (_trigger.Compare(_other))
                        return false;
                }

                return true;
            }

            bool HasExitedTrigger(Trigger _trigger)
            {
                for (int _i = 0; _i < triggerBuffer.Count; _i++)
                {
                    Trigger _other = triggerBuffer[_i];
                    if (_trigger.Compare(_other))
                        return false;
                }

                return true;
            }
        }

        // -----------------------

        /// <summary>
        /// Exits from all overlapping triggers.
        /// </summary>
        protected void ExitTriggers()
        {
            for (int _i = 0; _i < triggerOverlaps.Count; _i++)
            {
                triggerOverlaps[_i].OnExit(this);
            }
                
            triggerOverlaps.Clear();
        }
        #endregion

        #region Collision Callbacks
        /// <summary>
        /// Called after velocity has been applied on this object,
        /// that is after collision calculs.
        /// </summary>
        protected virtual bool OnAppliedVelocity(Vector3 _velocity, Vector3 _displacement, Buffer<RaycastHit> _buffer)
        {
            // Controller callback.
            if (useController && controller.OnAppliedVelocity(_velocity, _displacement))
                return true;

            // Reset speed if displacement is not sufficient compared to initial velocity.
            if (_displacement.sqrMagnitude < (SpeedVelocity.sqrMagnitude * .5f))
            {
                ResetSpeed();
            }

            // When using root motion, send encountered obstacle direction informations.
            if (usingRootMotion)
            {
                int _horizontal = 0;
                int _vertical = 0;
                RaycastHit _hit;

                // Iterate over collision buffer to find encountered obstacles.
                for (int _i = 0; _i < _buffer.Count; _i++)
                {
                    _hit = _buffer[_i];

                    if (Mathm.HaveDifferentSignAndNotNull(_velocity.x, _hit.normal.x) &&
                        !PhysicsUtility.IsGroundSurface(_hit.normal))
                    {
                        _horizontal = Mathm.Sign(_hit.normal.x);
                    }

                    if (_hit.normal.y != 0f)
                    {
                        _vertical = Mathm.Sign(_hit.normal.y);
                    }
                }

                // If didn't found any obstacle, perform a cast to get if against one.
                if ((_horizontal == 0) && (_velocity.x != 0f) &&
                    collider.DoCast(new Vector3(DreadfulCollider.MinCastLength * Mathf.Sign(_velocity.x), 0f, 0f), out _hit) && !PhysicsUtility.IsGroundSurface(_hit.normal))
                {
                    _horizontal = -Mathm.Sign(_velocity.x);
                }

                if ((_vertical == 0) && (_velocity.y != 0f) &&
                    collider.DoCast(new Vector3(0f, DreadfulCollider.MinCastLength * Mathf.Sign(_velocity.y), 0f), out _hit))
                {
                    _vertical = -Mathm.Sign(_velocity.y);
                }

                // Update animation obstacle informations.
                PlayHorizontalObstacle(_horizontal);
                PlayVerticalObstacle(_vertical);
            }

            return false;
        }

        /// <summary>
        /// Called when this object ground state has changed.
        /// </summary>
        protected virtual bool OnSetGrounded(bool _isGrounded)
        {
            // Controller callback.
            if (useController && controller.OnSetGrounded(_isGrounded))
                return true;

            // Reduce force when getting grounded.
            if (isGrounded)
            {
                force *= PhysicsSettings.I.OnGroundedForceMultiplier;
            }

            if (!_isGrounded)
            {
                
            }
            else
            {
                
            }

            return false;
        }
        #endregion

        #region MonoBehaviour
        protected virtual void Awake()
        {
            // Initializes object.
            collisionSystem = CollisionType.Get(this);
            collider.Initialize(CollisionMask);
        }
        #endregion
    }
}
