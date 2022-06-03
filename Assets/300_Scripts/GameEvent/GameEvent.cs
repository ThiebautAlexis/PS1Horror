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



}
