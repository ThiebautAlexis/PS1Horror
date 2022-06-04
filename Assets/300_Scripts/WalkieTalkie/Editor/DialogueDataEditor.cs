using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HorrorPS1
{
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : Editor
    {
        private SerializedProperty dialogueDatabaseProperty = null;
        private SerializedProperty dialogueLinesReferenceProperties = null;
        private SerializedProperty dialogueLinesProperties = null;
        private SerializedProperty portraitProperty = null;
        private string[] ids;
        private string[] lineValues;
        
        public override void OnInspectorGUI()
        {

            dialogueDatabaseProperty = serializedObject.FindProperty("database");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(dialogueDatabaseProperty, new GUIContent("Dialogue DataBase"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            if(dialogueDatabaseProperty == null)
            {
                EditorGUILayout.HelpBox("Cant find database", MessageType.Warning);
                return;
            }

            dialogueLinesProperties = serializedObject.FindProperty("LinesID");
            portraitProperty = serializedObject.FindProperty("Portrait");

            // LOAD REFERENCES
            SerializedObject _databaseReference = new SerializedObject(dialogueDatabaseProperty.objectReferenceValue);
            dialogueLinesReferenceProperties = _databaseReference.FindProperty("linesData");

            ids = new string[dialogueLinesReferenceProperties.arraySize];
            lineValues = new string[dialogueLinesReferenceProperties.arraySize];
            for (int i = 0; i < dialogueLinesReferenceProperties.arraySize; i++)
            {
                ids[i] = dialogueLinesReferenceProperties.GetArrayElementAtIndex(i).FindPropertyRelative("ID").stringValue;
                lineValues[i] = dialogueLinesReferenceProperties.GetArrayElementAtIndex(i).FindPropertyRelative("Line").stringValue;
            }
            // -------------
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(portraitProperty); 

            for (int i = 0; i < dialogueLinesProperties.arraySize; i++)
            {
                int _index = Mathf.Max(0, ids.ToList().IndexOf(dialogueLinesProperties.GetArrayElementAtIndex(i).stringValue));
                GUILayout.BeginHorizontal();
                _index = EditorGUILayout.Popup(_index, ids);
                dialogueLinesProperties.GetArrayElementAtIndex(i).stringValue = ids[_index]; 

                if (GUILayout.Button("x", GUILayout.Height(15), GUILayout.Width(15) ))
                {
                    if (dialogueLinesProperties.arraySize > 1)
                        dialogueLinesProperties.DeleteArrayElementAtIndex(dialogueLinesProperties.arraySize - 1);
                }
                GUILayout.EndHorizontal();
                GUILayout.TextArea(dialogueLinesReferenceProperties.GetArrayElementAtIndex(_index).FindPropertyRelative("Line").stringValue);
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            }
            if(dialogueLinesProperties.arraySize == 0)
            {
                dialogueLinesProperties.InsertArrayElementAtIndex(0);
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("+"))
            {
                dialogueLinesProperties.InsertArrayElementAtIndex(Mathf.Max(0, dialogueLinesProperties.arraySize - 1));
            }
            if(EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }


    }
}
