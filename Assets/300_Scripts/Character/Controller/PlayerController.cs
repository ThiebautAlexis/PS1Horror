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
        private static readonly string focusActionPath = "Player/Focus";

        private static readonly int speed_Hash = Animator.StringToHash("Speed");
        private static readonly int look_Hash = Animator.StringToHash("Look");


        #region Fields and Properties
        [Section("Inputs")]
        [SerializeField] private InputActionAsset inputActionAsset = null;

        private InputAction moveAction = null;
        private InputAction lookAction = null;
        private InputAction interactAction = null;
        private InputAction sprintAction = null;
        private InputAction focusAction = null;

        [Section("Components ")]
        [SerializeField] private CharacterTorchlight torchlight = null;
        [SerializeField] private WalkieTalkie walkieTalkie = null;
        [SerializeField] private Movable.Movable playerMovable = null;
        [SerializeField] private Animator animator = null;

        [Section("Physics")]
        [SerializeField] private LayerMask collisionMask = 0;

        [Section("Attributes")]
        [SerializeField, Enhanced] private PlayerAttributes playerAttributes = null;

        private bool isSprinting = false;
        private float sprintTimer = 0f;
        private Vector3 previousMovement = Vector3.zero;
        private Quaternion cameraReferenceRotation = Quaternion.identity;
        private Quaternion referenceRotation = Quaternion.identity;

        private bool hasToSetPosition = false;
        private Vector3 afterLoadingPosition = Vector3.zero;
        private Quaternion afterLoadingRotation = Quaternion.identity;
        #endregion

        #region Methods 
        protected override void OnActivation()
        {
            base.OnActivation();
            playerMovable.Controller = this;
            InGameState.Player = this;

            moveAction = inputActionAsset.FindAction(moveActionPath, true);
            moveAction.Enable();
            lookAction = inputActionAsset.FindAction(lookActionPath, true);
            lookAction.Enable();
            interactAction = inputActionAsset.FindAction(interactActionPath, true);
            interactAction.Enable();
            sprintAction = inputActionAsset.FindAction(sprintActionPath, true);
            sprintAction.Enable();
            focusAction = inputActionAsset.FindAction(focusActionPath, true);
            focusAction.Enable();

            InGameState.OnCameraChange += SetCurrentCamera;
            LoadingSceneState.OnEndSceneLoading += SetPlayerPosition;
            GameStatesManager.SetStateActivation(GameStatesManager.InGameState, true);
            SetCurrentCamera(InGameState.CurrentCamera);
        }

        protected override void OnDeactivation()
        {
            base.OnDeactivation();

            moveAction.Disable();
            lookAction.Disable();
            interactAction.Disable();
            sprintAction.Disable();
            focusAction.Disable();

            InGameState.OnCameraChange -= SetCurrentCamera;
            LoadingSceneState.OnEndSceneLoading -= SetPlayerPosition;

        }

        void IInputUpdate.Update()
        {
            if(GameStatesManager.currentGameState == GameStatesManager.InGameState)
            {
                Vector2 _movement = moveAction.ReadValue<Vector2>();
                playerMovable.AddHorizontalMovement(_movement.x, false);
                playerMovable.AddForwardMovement(_movement.y);

                // Check here if the sprint input is triggered
                isSprinting = !isSprinting && (sprintTimer / playerAttributes.SprintLimit) > playerAttributes.SprintThreshold ? // if is in cooldown
                              false :                                                                                           // then false
                              sprintAction.IsPressed();                                                                         // else check input

                ApplySprint();


                if (focusAction.WasPressedThisFrame())
                    torchlight.FocusTorchLight();
                else if (focusAction.WasReleasedThisFrame())
                    torchlight.UnfocusTorchLight();
                
            }

            if (interactAction.WasPressedThisFrame())
            {
                if (GameStatesManager.currentGameState == GameStatesManager.InGameState)
                    torchlight.TryInteraction();    
                else if (GameStatesManager.currentGameState == GameStatesManager.WalkieTalkieState)
                        walkieTalkie.SkipReading();
                else if(GameStatesManager.currentGameState == GameStatesManager.InfoState)
                        InfoState.CloseInfo();
            }

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
            // staminaGauge.fillAmount =  Mathf.MoveTowards(staminaGauge.fillAmount, 1f - (sprintTimer / playerAttributes.SprintLimit), Time.deltaTime);
        }

        private void SetCurrentCamera(Cinemachine.CinemachineVirtualCamera _cam)
        {
            cameraReferenceRotation = _cam.transform.rotation;
            if (referenceRotation == Quaternion.identity)
                referenceRotation = cameraReferenceRotation;
        }

        public void SetAfterLoadingTransform(Transform _transform)
        {
            afterLoadingPosition = _transform.position;
            afterLoadingRotation = transform.rotation;
            hasToSetPosition = true;
        }

        /// <summary>
        /// Do not use this method from another script.
        /// It is called from the <see cref="LoadingSceneState.OnEndSceneLoading"/> event
        /// </summary>
        private void SetPlayerPosition()
        {
            if (hasToSetPosition)
            {
                transform.position = afterLoadingPosition;
                transform.rotation = afterLoadingRotation;
                hasToSetPosition = false;
            }
            else Debug.LogWarning("Be sure to call the method SetAfterLoadingTransform() while loading a new scene");
        }

        #region IMovable Controller
        public bool OnPlayHorizontalObstacle(int _hash, int _value) => false;

        public bool OnPlayVerticallObstacle(int _hash, int _value) => false;
        public bool OnApplyGravity() => false;

        private Quaternion previousAimedAngle = Quaternion.identity;
        public bool OnComputeVelocity(ref Vector3 _force, ref Vector3 _instantForce, ref Vector3 _movement)
        {

            if (Mathf.Abs(_movement.x) <= Mathf.Epsilon && Mathf.Abs(_movement.z) <= Mathf.Epsilon)
            {
                previousMovement = _movement;
                return false;
            }

            // Sprint
            if (isSprinting)
                _movement *= playerAttributes.SprintMultiplier;

            // Rotation 
            if (cameraReferenceRotation != referenceRotation && Mathf.Abs(previousMovement.x) <= Mathf.Epsilon && Mathf.Abs(previousMovement.z) <= Mathf.Epsilon)
                referenceRotation = cameraReferenceRotation;

            _movement = referenceRotation * _movement;

            _movement.y = 0f;
            Quaternion _aimedAngle = Quaternion.LookRotation(_movement, Vector3.up);
            previousAimedAngle = _aimedAngle;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _aimedAngle, playerAttributes.RotationSpeed * Time.deltaTime);
            if (Mathf.Abs(_aimedAngle.eulerAngles.y - transform.eulerAngles.y) > playerAttributes.RotationThreshold)
            {
                _movement = Vector3.zero;
            }
            previousMovement = _movement;
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