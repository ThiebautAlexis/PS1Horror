using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public abstract class GameEvent : ScriptableObject
    {
        #region Methods
        public abstract void CallEvent();
        #endregion
    }

    [CreateAssetMenu(fileName = "Event_", menuName = "PS1 Horror Game/Events/Open door Event", order = 150)]
    public class OpenDoorEvent : GameEvent
    {
        public override void CallEvent()
        {
            Debug.Log("Implement behaviour here");
        }
    }

}
