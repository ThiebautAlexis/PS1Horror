using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class GETriggerInteraction : GameEventTrigger, IInteractable
    {
        #region Methods 

        public void CancelInteraction()
        {
        }

        public void Interact()
        {
            TriggerEvents();
        }
        #endregion
    }
}
