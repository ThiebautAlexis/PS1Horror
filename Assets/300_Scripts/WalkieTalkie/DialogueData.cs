using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Dialogue_", menuName = "PS1 Horror Game/Data/Dialogue", order = 150)]
    public class DialogueData : ScriptableObject
    {
        public DialogueLine[] DialogueLines = new DialogueLine[] { };
    }

    [Serializable]
    public class DialogueLine
    {
        public Sprite Portrait = null;
        public string[] Lines = new string[] { };
    }
}
