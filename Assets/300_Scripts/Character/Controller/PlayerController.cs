using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using HorrorPS1.Core;
using HorrorPS1.Movable;

namespace HorrorPS1
{
    public class PlayerController : HorrorBehaviour, IInputUpdate, IMovableController
    {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Input;

        public CollisionSystemType CollisionType => CollisionSystemType.Simple;

        public int CollisionMask => collisionMask;

        private static readonly string moveActionPath = "Player/Move";
        private static readonly string lookActionPath = "Player/Look";
        private static readonly string interactActionPath = "Player/Interact";
        private static readonly string sprintActionPath = "Player/Sprint";

        private static readonly int speed_Hash = Animator.StringToHash("Speed");
        private static readonly int look_Hash = Animator.StringToHash("Look");


        #region Fields and Properties
        [Section("Inputs")]
        [SerializeField] private InputActionAsset inputActionAsset = null;

        private InputAction moveAction = null;
        private InputAction lookAction = null;
        private InputAction interactAction = null;
        private InputAction sprintAction = null;

        [Section("Components ")]
        [SerializeField] private CharacterTorchlight torchlight = null;
        [SerializeField] private Movable.Movable playerMovable = null;
        [SerializeField] private Animator animator = null;

        [Section("Physics")]
        [SerializeField] private LayerMask collisionMask = 0;

        [Section("Camera")]
        [SerializeField, Enhanced] private bool useDirectionFromCamera = false;
        [SerializeField, Enhanced, ShowIf("useDirectionFromCamera")] private Camera currentCamera = null;

        [Section("Attributes")]
        [SerializeField, Enhanced] private PlayerAttributes playerAttributes = null;

        [Section("UI")]
        [SerializeField, Enhanced] private UnityEngine.UI.Image staminaGauge = null;

        private bool isSprinting = false;
        private float sprintTimer = 0f;
        #endregion

        #region Methods 
        protected override void OnActivation()
        {
            base.OnActivation();
            playerMovable.Controller = this;

            moveAction = inputActionAsset.FindAction(moveActionPath, true);
            moveAction.Enable();
            lookAction = inputActionAsset.FindAction(lookActionPath, true);
            lookAction.Enable();
            interactAction = inputActionAsset.FindAction(interactActionPath, true);
            interactAction.Enable();
            sprintAction = inputActionAsset.FindAction(sprintActionPath, true);
            sprintAction.Enable();

            GameState.OnCameraChange += SetCurrentCamera;
        }

        protected override void OnDeactivation()
        {
            base.OnDeactivation();
            moveAction.Disable();
            lookAction.Disable();
            interactAction.Disable();
            sprintAction.Disable();


            GameState.OnCameraChange -= SetCurrentCamera;
        }

        void IInputUpdate.Update()
        {
            Vector2 _movement = moveAction.ReadValue<Vector2>();
            playerMovable.AddHorizontalMovement(_movement.x, false);
            playerMovable.AddForwardMovement(_movement.y);

            // Check here if the sprint input is triggered
            //if(!isSprinting && (sprintTimer/playerAttributes.SprintLimit) > playerAttributes.SprintThreshold )
            //{
            //    this.Log("in cooldown");
            //    isSprinting = false;
            //}
            //else isSprinting = sprintAction.IsPressed();

            isSprinting = !isSprinting && (sprintTimer / playerAttributes.SprintLimit) > playerAttributes.SprintThreshold ? // if is in cooldown
                          false :                                                                                           // then false
                          sprintAction.IsPressed();                                                                         // else check input

            ApplySprint();


            if (interactAction.WasPressedThisFrame())
                torchlight.FocusTorchLight();
            else if (interactAction.WasReleasedThisFrame())
                torchlight.UnfocusTorchLight();
        }

        private void ApplySprint()
        {
            if (isSprinting)
            {
                sprintTimer = Mathf.Min(playerAttributes.SprintLimit, sprintTimer + Time.deltaTime);
                if (sprintTimer == playerAttributes.SprintLimit)
                {
                    isSprinting = false;
                }
            }
            else
            {
                sprintTimer = Mathf.Max(0, sprintTimer - Time.deltaTime);
            }
            staminaGauge.fillAmount =  Mathf.MoveTowards(staminaGauge.fillAmount, 1f - (sprintTimer / playerAttributes.SprintLimit), Time.deltaTime);
        }

        private void SetCurrentCamera(Camera _cam) => currentCamera = _cam;

        #region IMovable Controller
        public bool OnPlayHorizontalObstacle(int _hash, int _value) => false;

        public bool OnPlayVerticallObstacle(int _hash, int _value) => false;
        public bool OnApplyGravity() => false;

        private Quaternion previousAimedAngle = Quaternion.identity;
        public bool OnComputeVelocity(ref Vector3 _force, ref Vector3 _instantForce, ref Vector3 _movement)
        {

            if (Mathf.Abs(_movement.x) <= Mathf.Epsilon && Mathf.Abs(_movement.z) <= Mathf.Epsilon)
            {
                return false;
            }

            // Sprint
            if (isSprinting)
                _movement *= playerAttributes.SprintMultiplier;

            // Rotation 
            if (useDirectionFromCamera)
                _movement = currentCamera.transform.rotation * _movement;
            _movement.y = 0f;
            Quaternion _aimedAngle = Quaternion.LookRotation(_movement, Vector3.up);
            previousAimedAngle = _aimedAngle;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _aimedAngle, playerAttributes.RotationSpeed * Time.deltaTime);
            if (Mathf.Abs(_aimedAngle.eulerAngles.y - transform.eulerAngles.y) > playerAttributes.RotationThreshold)
            {
                _movement = Vector3.zero;
            }           
            return false;
        }

        public bool OnGetUp() => false;

        public bool OnAppliedVelocity(Vector3 _velocity, Vector3 _displacement)
        {           
            animator.SetFloat(speed_Hash, (playerMovable.Speed/playerMovable.MaxSpeed) * (isSprinting ? 2 : 1));
            return false;
        }

        public bool OnSetGrounded(bool _isGrounded) => false;
        #endregion
        
        #endregion
    }
}