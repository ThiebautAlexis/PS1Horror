using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Player Inventory", menuName = "PS1 Horror Game/Player/Inventory", order = 150)]
    public class CharacterInventory : ScriptableObject
    {
        #region Content
        public int InventoryValue = 0;

        public static readonly int SaltValue        = 1;
        public static readonly int StairsKeyValue   = 2;
        #endregion

        #region Methods 

        #endregion
    }
}
