using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class TorchLightData : ScriptableObject
    {
        #region Content
        [Header("Light Data")]
        public float BaseIntensity = 1.0f;
        public float FocusIntensity = 2.0f;

        public float BaseConeAngle = 70.0f;
        public float FocusConeAngle = 25.0f;

        public float FocusAngleDuration = 1.0f;
        public Ease FocusAngleEase = Ease.Linear;           
        
        public float FocusIntensityDuration = 1.0f;
        public Ease FocusIntensityEase = Ease.Linear;        
        #endregion
    }
}
