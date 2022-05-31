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
        [SerializeField, ReadOnly] private bool canInteract = false;

        private IInteractable currentInteractable = null;
        private float interactionTimer = 0.0f;
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
                canInteract = true;
                /// Add Callbacks here.
            }
        }

        [Button(SuperColor.Red)]
        public void UnfocusTorchLight()
        {
            canInteract = false;
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

        public bool CastInteraction()
        {
            int _amount = Physics.RaycastNonAlloc(light.transform.position, light.transform.forward, interactionArray, light.range , data.InteractionLayer);
            if(_amount > 0)
                return interactionArray[0].collider.TryGetComponent(out currentInteractable);
            return false;

        }
        
        private void Update()
        {
            if(canInteract && CastInteraction())
            {
                interactionTimer += Time.deltaTime;
                if(interactionTimer >= data.InteractionDuration)
                {
                    interactionTimer = 0.0f;
                    currentInteractable.Interact();
                }
            }
        }
        #endregion
    }
}
