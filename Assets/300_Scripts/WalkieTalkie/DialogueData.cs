using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Dialogue_", menuName = "PS1 Horror Game/Dialogues/Dialogue Asset", order = 150)]
    public class DialogueData : ScriptableObject
    {
        [SerializeField] private DialogueDatabase database;
        public Sprite Portrait = null;
        public string[] LinesID = new string[] { };


        public string GetLine(int _index)
        {
            for (int i = 0; i < database.linesData.Length; i++)
            {
                if (LinesID[_index] == database.linesData[i].ID)
                    return database.linesData[i].Line;

            }
            return string.Empty;
        }
    }
}
