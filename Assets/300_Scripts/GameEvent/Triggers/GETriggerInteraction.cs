using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class GETriggerInteraction : GameEventTrigger, IInteractable
    {
        #region Methods 
        public void Interact()
        {
            TriggerEvents();
        }
        #endregion
    }
}
