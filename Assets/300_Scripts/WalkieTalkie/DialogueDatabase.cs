using EnhancedEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Lines Database", menuName = "PS1 Horror Game/Dialogues/Database")]
    public class DialogueDatabase : ScriptableObject
    {
        #region Fields and Properties
        [SerializeField] private string csvPath = string.Empty;
        [SerializeField] public LineData[] linesData = new LineData[] { };
        #endregion

        #region Methods 
        [Button]
        public void GenerateDatas()
        {
            if (string.IsNullOrEmpty(csvPath) || !File.Exists(csvPath))
            {
                Debug.LogWarning("Path is not set or incorrect");
                return;
            }
            string[] _lines = File.ReadAllLines(csvPath);
            List<LineData> _linesData = new List<LineData>();
            for (int i = 1; i < _lines.Length; i++)
            {
                string[] _split = _lines[i].Split('\t');
                if (string.IsNullOrEmpty(_split[0].Trim()))
                    continue;
                _linesData.Add(new LineData(_split)) ;
            }
            linesData = _linesData.ToArray();
        }
        #endregion
    }

    [Serializable]
    public class LineData
    {
        public string ID;
        public string Line;
        public string Annotation;

        public LineData(string[] _data)
        {
            ID = _data[0];
            Line = _data[1];
            if(_data.Length > 2) Annotation = _data[2];
        }
    }
}
