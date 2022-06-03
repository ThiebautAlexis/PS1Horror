using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Event_", menuName = "PS1 Horror Game/Events/Open door Event", order = 150)]
    public class OpenDoorEvent : GameEvent
    {
        //[SerializeField] private SceneData targetSceneData = null;
        //[SerializeField] private int UnlockedIndex = -1;

        public override void CallEvent()
        {
            Debug.Log("Implement behaviour here");
        }
    }
}
