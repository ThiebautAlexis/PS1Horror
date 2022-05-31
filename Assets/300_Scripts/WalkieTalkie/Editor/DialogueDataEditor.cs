using EnhancedEditor;
using EnhancedEditor.Editor;
using System;
using UnityEngine;
using UnityEditor;

namespace HorrorPS1
{
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : Editor
    {
        private SerializedProperty dialogueLinesProperties = null;

        public override void OnInspectorGUI()
        {
            dialogueLinesProperties = serializedObject.FindProperty("DialogueLines");
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < dialogueLinesProperties.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(dialogueLinesProperties.GetArrayElementAtIndex(i)); 
                if (GUILayout.Button("x", GUILayout.Height(15), GUILayout.Width(15) ))
                {
                    dialogueLinesProperties.DeleteArrayElementAtIndex(dialogueLinesProperties.arraySize - 1);
                }
                GUILayout.EndHorizontal();
            }
            if(GUILayout.Button("+"))
            {
                dialogueLinesProperties.InsertArrayElementAtIndex(Mathf.Max(0, dialogueLinesProperties.arraySize - 1));
            }
            if(EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }


    }
}
