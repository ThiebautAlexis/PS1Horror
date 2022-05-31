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
        #endregion

        #region Methods 
        public void FocusTorchLight()
        {
            if(focusingSequence.IsActive())
            {
                focusingSequence.Kill();
            }
            focusingSequence = DOTween.Sequence();
            {
                DOTween.To(angle => light.spotAngle = angle, data.BaseConeAngle, data.FocusConeAngle, data.FocusAngleDuration).SetEase(data.FocusAngleEase);
                DOTween.To(intensity => light.intensity = intensity, data.BaseIntensity, data.FocusIntensity, data.FocusIntensityDuration).SetEase(data.FocusIntensityEase);
            };
        }

        public void UnfocusTorchLight()
        {
            if(focusingSequence.IsActive())
            {
                focusingSequence.Kill();
            }
            focusingSequence = DOTween.Sequence();
            {
                DOTween.To(angle => light.spotAngle = angle, data.FocusConeAngle, data.BaseConeAngle, data.FocusAngleDuration).SetEase(data.FocusAngleEase);
                DOTween.To(intensity => light.intensity = intensity, data.FocusIntensity, data.BaseIntensity, data.FocusIntensityDuration).SetEase(data.FocusIntensityEase);
            };
        }
        #endregion
    }
}
