using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class CharacterTorchlight : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField, Enhanced, Required] private new Light light = null;
        [SerializeField, Enhanced, Required] private TorchLightData data = null;
        private Sequence focusingSequence = null;

        private IInteractable currentInteractable = null;
        private static readonly RaycastHit[] interactionArray = new RaycastHit[1];
        #endregion

        #region Methods 
        [Button(SuperColor.Green)]
        public void FocusTorchLight()
        {
            if(focusingSequence.IsActive())
            {
                focusingSequence.Kill(false);
            }
            focusingSequence = DOTween.Sequence();
            {
                focusingSequence.Join(DOTween.To(angle => light.spotAngle = angle, light.spotAngle, data.FocusConeAngle, data.FocusAngleDuration).SetEase(data.FocusAngleEase));
                focusingSequence.Join(DOTween.To(intensity => light.intensity = intensity, light.intensity, data.FocusIntensity, data.FocusIntensityDuration).SetEase(data.FocusIntensityEase));
                focusingSequence.AppendCallback(OnComplete);    
            };

            void OnComplete()
            {
                /// Add Callbacks here.
            }
        }

        [Button(SuperColor.Red)]
        public void UnfocusTorchLight()
        {
            if(focusingSequence.IsActive())
            {
                focusingSequence.Kill(false);
            }
            focusingSequence = DOTween.Sequence();
            {
                DOTween.To(angle => light.spotAngle = angle, light.spotAngle, data.BaseConeAngle, data.FocusAngleDuration).SetEase(data.FocusAngleEase);
                DOTween.To(intensity => light.intensity = intensity, light.intensity, data.BaseIntensity, data.FocusIntensityDuration).SetEase(data.FocusIntensityEase);
            };
        }

        private bool CastInteraction(out IInteractable _interactable)
        {
#if UNITY_EDITOR
            Debug.DrawRay(light.transform.position, light.transform.forward, Color.yellow);
#endif
            int _amount = Physics.RaycastNonAlloc(light.transform.position, light.transform.forward, interactionArray, 1f , data.InteractionLayer);
            if(_amount > 0)
            {
                return interactionArray[0].collider.TryGetComponent(out _interactable);
            }
            _interactable = null;
            return false;

        }

        public void TryInteraction()
        {
            if (CastInteraction(out IInteractable _interactable))
            {
                _interactable.Interact();
            }
        }
        
        private void Update()
        {
            /*
            if(canInteract && CastInteraction(out IInteractable _interactable))
            {
                if (_interactable == currentInteractable)
                    return;

                if(currentInteractable != null)
                {
                    currentInteractable.CancelInteraction();
                }
                currentInteractable = _interactable;
                currentInteractable.Interact();
            }
            else if(currentInteractable != null)
            {
                currentInteractable.CancelInteraction();
                currentInteractable = null;
            }
            */
        }
        #endregion
    }
}
