using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using HorrorPS1.Core;

namespace HorrorPS1
{
    public class PlayerController : HorrorBehaviour, IInputUpdate
    {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Input;

        private static readonly string moveActionPath = "Player/Move";
        private static readonly string lookActionPath = "Player/Look";
        private static readonly string interactActionPath = "Player/Interact";
        private static readonly string sprintActionPath = "Player/Sprint";

        private static readonly int speed_Hash = Animator.StringToHash("Speed");
        private static readonly int look_Hash = Animator.StringToHash("Look");


        #region Fields and Properties
        [Header("Inputs")]
        [SerializeField] private PlayerInputData inputs = null;
        [SerializeField] private InputActionAsset inputActionAsset = null;

        private InputAction moveAction      = null;
        private InputAction lookAction      = null;
        private InputAction interactAction  = null;

        [Header("Components ")]
        [SerializeField] private CharacterTorchlight torchlight = null;
        [SerializeField] private Movable.Movable playerMovable = null;
        [SerializeField] private Animator animator = null;
        #endregion

        #region Methods 
        protected override void OnActivation()
        {
            base.OnActivation();
            moveAction = inputActionAsset.FindAction(moveActionPath, true);
            moveAction.Enable();            
            lookAction = inputActionAsset.FindAction(lookActionPath, true);
            lookAction.Enable();            
            interactAction = inputActionAsset.FindAction(interactActionPath, true);
            interactAction.Enable();
        }

        void IInputUpdate.Update()
        {
            Vector2 _movement = moveAction.ReadValue<Vector2>();
            playerMovable.AddHorizontalMovement(_movement.x);
            playerMovable.AddForwardMovement(_movement.y);

            // Check here if the sprint input is triggered
            // Then send it into the animator
            animator.SetInteger(speed_Hash, (_movement.magnitude > 0 ? 1 : 0));

            if (interactAction.WasPressedThisFrame())
                torchlight.FocusTorchLight();
            else if (interactAction.WasReleasedThisFrame())
                torchlight.UnfocusTorchLight();
        }
        #endregion
    }
}
