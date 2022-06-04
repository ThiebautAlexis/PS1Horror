using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InteractableDebug : MonoBehaviour, IInteractable
    {
        #region Methods 
        [SerializeField] private string displayedText = "This is a nice object!";

        void IInteractable.Interact()
        {
            InfoState.DisplayInteractionInfo(displayedText);
        }
        #endregion
    }
}
