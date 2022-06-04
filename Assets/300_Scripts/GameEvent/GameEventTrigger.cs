using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public abstract class GameEventTrigger : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] protected bool isTriggeredOnce = true;
        [SerializeField] protected GameEvent[] events = new GameEvent[] { }; 
        #endregion

        #region Methods 
        protected void TriggerEvents()
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].CallEvent(this);
            }
            if (isTriggeredOnce)
                enabled = false;
        }
        #endregion
    }
}
