using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InteractableDebug : MonoBehaviour, IInteractable
    {
        #region Fields and Properties
        [SerializeField] private float interactionTimer  = 10.0f;

        private Sequence interactionSequence;
        #endregion

        #region Methods 

        void IInteractable.Interact()
        {
            gameObject.SetActive(false);
            //if (interactionSequence.IsActive())
            //    interactionSequence.Kill(false);
            //interactionSequence = DOTween.Sequence();
            //{
            //    interactionSequence.Join(DOVirtual.DelayedCall(interactionTimer, ApplyInteraction));
            //}

            //void ApplyInteraction()
            //{
            //}
        }

        //void IInteractable.CancelInteraction()
        //{
        //    if (interactionSequence.IsActive())
        //        interactionSequence.Kill(false);
        //}
        #endregion
    }
}
