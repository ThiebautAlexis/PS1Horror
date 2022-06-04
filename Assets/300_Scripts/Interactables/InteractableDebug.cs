using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InteractableDebug : MonoBehaviour, IInteractable
    {
        #region Methods 

        void IInteractable.Interact()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
