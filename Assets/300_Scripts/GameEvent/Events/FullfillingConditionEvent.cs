using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "ConditionEvent_", menuName = "PS1 Horror Game/Events/Condition Event", order = 150)]
    public class FullfillingConditionEvent : GameEvent
    {
        #region Fields and Properties
        [Section("Inventory Event")]
        [HelpBox("Be careful! You can select multiple elements in this enum field", MessageType.Warning, false)]
        [SerializeField, Enhanced] private ConditionElements inventoryElement;
        [SerializeField, Enhanced, Space] private InventoryAction action = InventoryAction.Add;
        #endregion

        #region Methods 

        #endregion
        public override void CallEvent(GameEventTrigger _trigger)
        {
            switch (action)
            {
                case InventoryAction.Add:
                    GameState.FullfillCondition(inventoryElement);
                    break;
                case InventoryAction.Remove:
                    GameState.RemoveCondition(inventoryElement);
                    break;
                default:
                    break;
            }
        }

        private enum InventoryAction
        {
            Add, 
            Remove
        }
    }

}
