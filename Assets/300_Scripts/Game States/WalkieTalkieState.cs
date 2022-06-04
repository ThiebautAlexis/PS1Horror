using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class WalkieTalkieState : GameState
    {
        public override int Priority { get; protected set; } = 2;
        public override bool isActive { get; set; } = false;

        #region Fields and Properties
        public static event Action<DialogueData> OnReadDialogueLine;
        public static event Action OnSkipDialogueLine;
        public static event Action OnEndDialogue;
        #endregion

        #region Methods 
        public static void ReadDialogueLine(DialogueData _line) => OnReadDialogueLine?.Invoke(_line);
        public static void SkipDialogueLine() => OnSkipDialogueLine?.Invoke();
        public static void EndDialogue() => OnEndDialogue?.Invoke();
        #endregion
    }
}
