using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public abstract class GameState
    {
        public abstract int Priority { get; protected set; }
        public abstract bool isActive { get; set; }
    }
}
