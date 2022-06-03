using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Event_", menuName = "PS1 Horror Game/Events/Instantiate Event", order = 150)]
    public class InstantiateEvent : GameEvent
    {
        #region Fields and Properties
        [SerializeField] private GameObject instantiedPrefab = null;
        #endregion

        #region Methods 

        #endregion
        public override void CallEvent(GameEventTrigger _trigger)
        {
            Instantiate(instantiedPrefab, _trigger.transform.position, _trigger.transform.rotation);
        }
    }
}
