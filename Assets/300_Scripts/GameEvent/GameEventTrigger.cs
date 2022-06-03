using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HorrorPS1
{
    public abstract class GameEventTrigger : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private bool isTriggeredOnce = true;
        [SerializeField] private GameEvent[] events = new GameEvent[] { };
        [SerializeField] private UnityEvent sceneEvents = new UnityEvent();
        [SerializeField] private ConditionElements conditions = 0; 
        #endregion

        #region Methods 
        protected void TriggerEvents()
        {
            if(conditions > 0 && (GameState.FullfilledConditions & conditions) == 0)
                return;

            for (int i = 0; i < events.Length; i++)
            {
                events[i].CallEvent(this);
            }
            sceneEvents?.Invoke();

            if (isTriggeredOnce)
                enabled = false;
        }
        #endregion
    }
}
