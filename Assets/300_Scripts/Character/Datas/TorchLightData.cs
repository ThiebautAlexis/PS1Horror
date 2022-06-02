using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "TorchLightData", menuName = "PS1 Horror Game/Data/Player/Torchlight", order = 150)]
    public class TorchLightData : ScriptableObject
    {
        #region Content
        [Space, Header("Angle")]
        public float BaseConeAngle = 70.0f;
        public float FocusConeAngle = 25.0f;
        public float FocusAngleDuration = 1.0f;
        public Ease FocusAngleEase = Ease.Linear;           

        [Header("Intensity")]
        public float BaseIntensity = 1.0f;
        public float FocusIntensity = 2.0f;
        public float FocusIntensityDuration = 1.0f;
        public Ease FocusIntensityEase = Ease.Linear;

        [Header("Interactions")]
        public float InteractionDuration = .5f;
        public LayerMask InteractionLayer = new LayerMask();

        #endregion
    }
}
