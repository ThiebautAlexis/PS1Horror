using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InfoState : GameState
    {
        public override int Priority { get; protected set; } = 2;
        public override bool isActive { get; set; } = false;

        #region Events
        public static event Action<string> OnDisplayInfo;
        public static event Action OnCloseInfo;
        #endregion

        #region Methods 
        public static void DisplayInteractionInfo(string _info)
        {
            GameStatesManager.SetStateActivation(GameStatesManager.InfoState, true);
            OnDisplayInfo?.Invoke(_info);
        }

        public static void CloseInfo()
        {
            OnCloseInfo?.Invoke();
            GameStatesManager.SetStateActivation(GameStatesManager.InfoState, false);
        }
        #endregion
    }
}
