using EnhancedEditor.Editor;
using System;
using UnityEngine;
using UnityEditor;

namespace HorrorPS1
{
    [CustomPropertyDrawer(typeof(DialogueLine))]
    public class DialogueLinePropertyDrawer : EnhancedPropertyEditor
    {
        private SerializedProperty portraitProperty = null;
        private SerializedProperty linesProperties = null;

        private readonly float portraitSize = 50;
        private readonly float lineHeight = 30;

        protected override float GetDefaultHeight(SerializedProperty _property, GUIContent _label)
        {
            linesProperties = _property.FindPropertyRelative("Lines");
            return portraitSize + linesProperties.arraySize * (lineHeight + EditorGUIUtility.singleLineHeight) + EditorGUIUtility.singleLineHeight * 3;
        }

        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            portraitProperty = _property.FindPropertyRelative("Portrait");
            linesProperties = _property.FindPropertyRelative("Lines");
            Rect _r = new Rect(_position.x, _position.y, _position.width, portraitSize);
            EditorGUI.PropertyField(_r, portraitProperty);
            _r.y += portraitSize;
            
            for (int i = 0; i < linesProperties.arraySize; i++)
            {
                _r.x = _position.x;
                _r.height = lineHeight;
                _r.y += EditorGUIUtility.singleLineHeight;
                _r.width = _position.width - 10;
                linesProperties.GetArrayElementAtIndex(i).stringValue = EditorGUI.TextField(_r, linesProperties.GetArrayElementAtIndex(i).stringValue);

                _r.x += _r.width;
                _r.height = _r.width = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(_r, "x"))
                    linesProperties.DeleteArrayElementAtIndex(i);

                _r.y += lineHeight;
            }
            _r.y += EditorGUIUtility.singleLineHeight;
            _r.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(_r, "+"))
                linesProperties.InsertArrayElementAtIndex(Mathf.Max(0, linesProperties.arraySize - 1));

            _r.y += EditorGUIUtility.singleLineHeight;
            _r.x = _position.x;
            _r.width = _position.width;
            _r.height = 5;

            Color _c = GUI.color;
            GUI.color = Color.black;
            GUI.Box(_r, string.Empty);
            GUI.color = _c;

            return GetDefaultHeight(_property, _label);
        }
    }
}
