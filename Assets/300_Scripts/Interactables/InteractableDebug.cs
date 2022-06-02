using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InteractableDebug : MonoBehaviour, IInteractable
    {
        #region Fields and Properties

        #endregion

        #region Methods 
        public void Interact()
        {
            Debug.Log("Interact!");
            gameObject.SetActive(false);
        }
        #endregion
    }
}
