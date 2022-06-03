using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class GETriggerDebug : GameEventTrigger
    {
        [Button]
        private void DebugEvent()
        {
            TriggerEvents();
        }
    }
}
