using EnhancedEditor;
using System;
using UnityEngine;
using DG.Tweening;

namespace HorrorPS1
{
    public class FadingScreenUI : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private CanvasGroup group = null;
        [SerializeField] private float fadingDuration = .5f; 
        private Sequence fadingSequence = null; 
        #endregion

        #region Methods 
        private void OnEnable()
        {
            LoadingSceneState.OnStartSceneLoading += FadeIn;
            LoadingSceneState.OnEndSceneLoading += FadeOut;
        }

        private void OnDisable()
        {
            LoadingSceneState.OnStartSceneLoading -= FadeIn;
            LoadingSceneState.OnEndSceneLoading -= FadeOut;
        }

        private void FadeIn()
        {
            if (fadingSequence.IsActive())
                fadingSequence.Kill(false);

            fadingSequence = DOTween.Sequence();
            {
                fadingSequence.Join(DOTween.To(UpdateAlphaValue, group.alpha, 1, fadingDuration));
            }
        }

        private void FadeOut()
        {
            if (fadingSequence.IsActive())
                fadingSequence.Kill(false);

            fadingSequence = DOTween.Sequence();
            {
                fadingSequence.Join(DOTween.To(UpdateAlphaValue, group.alpha, 0, fadingDuration));
            }
        }

        private void UpdateAlphaValue(float _value) => group.alpha = _value;
        #endregion
    }
}
