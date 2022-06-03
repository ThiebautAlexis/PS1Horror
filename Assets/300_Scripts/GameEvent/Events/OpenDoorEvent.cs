using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "OpenDoorEvent_", menuName = "PS1 Horror Game/Events/Open door Event", order = 150)]
    public class OpenDoorEvent : GameEvent
    {
        [Section("Open Door Event")]
        [SerializeField, Enhanced, EnhancedEditor.Range(-1,9)] private int unlockedIndex = -1;
        [SerializeField] private SceneData targetSceneData = null;

        public override void CallEvent(GameEventTrigger _trigger)
        {
            targetSceneData.UnlockDoor(unlockedIndex);
        }
    }
}
