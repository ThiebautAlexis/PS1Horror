using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class GETrigger : GameEventTrigger
    {
        [SerializeField] private TriggerState activationState = TriggerState.None;

        private void OnTriggerEnter(Collider other)
        {
            if (activationState == TriggerState.OnEnter)
                TriggerEvents();
        }

        private void OnTriggerExit(Collider other)
        {
            if (activationState == TriggerState.OnExit)
                TriggerEvents();
        }

        private void OnTriggerStay(Collider other)
        {
            if(activationState == TriggerState.OnStay)
            {
                Debug.Log("This feature is not implemented yet");
            }
        }

        private enum TriggerState
        {
            None,
            OnEnter,
            OnStay,
            OnExit
        }
    }
}
