using System.Collections.Generic;
using Core.Quests;
using SceneManagement.Enums;
using UnityEditor;
using UnityEngine;
using static SceneManagement.LevelChangeObserver;

[CustomEditor(typeof(SceneWithTestsID))]
public class SceneWithTestsIDEditor : Editor
{
    private Dictionary<allScenes, string> numberInputs = new Dictionary<allScenes, string>();

    public override void OnInspectorGUI()
    {
        SceneWithTestsID sceneWithTestsID = (SceneWithTestsID)target;

        SerializedObject serializedObject = new SerializedObject(sceneWithTestsID);
        SerializedProperty sceneDataListProperty = serializedObject.FindProperty("sceneDataList");

        for (int i = 0; i < sceneDataListProperty.arraySize; i++)
        {
            SerializedProperty sceneDataProperty = sceneDataListProperty.GetArrayElementAtIndex(i);
            DrawSceneDataElement(sceneDataProperty, i);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        if (GUILayout.Button("Add New Scene Data"))
        {
            sceneDataListProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
        }

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawSceneDataElement(SerializedProperty sceneDataProperty, int index)
    {
        EditorGUILayout.BeginVertical("box");

        SerializedProperty sceneProperty = sceneDataProperty.FindPropertyRelative("scene");
        EditorGUILayout.PropertyField(sceneProperty, new GUIContent("Scene"));

        SerializedProperty numbersProperty = sceneDataProperty.FindPropertyRelative("numbers");
        EditorGUILayout.PropertyField(numbersProperty, new GUIContent("Id\'s"), true);

        allScenes scene = (allScenes)sceneProperty.enumValueIndex;
        if (!numberInputs.ContainsKey(scene))
        {
            numberInputs[scene] = "";
        }

        numberInputs[scene] = EditorGUILayout.TextField("Add Id\'s", numberInputs[scene]);
        if (GUILayout.Button("Add Id\'s"))
        {
            AddNumbersToList(numbersProperty, numberInputs[scene]);
            numberInputs[scene] = "";
        }

        if (GUILayout.Button("Remove This scene"))
        {
            SerializedObject targetObject = new SerializedObject(target);
            SerializedProperty sceneDataListProperty = targetObject.FindProperty("sceneDataList");
            sceneDataListProperty.DeleteArrayElementAtIndex(index);
            targetObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.EndVertical();
    }

    private void AddNumbersToList(SerializedProperty numbersProperty, string numberInput)
    {
        HashSet<int> existingNumbers = new HashSet<int>();
        for (int i = 0; i < numbersProperty.arraySize; i++)
        {
            existingNumbers.Add(numbersProperty.GetArrayElementAtIndex(i).intValue);
        }

        string[] numberStrings = numberInput.Split(',');
        foreach (string numberString in numberStrings)
        {
            if (int.TryParse(numberString.Trim(), out int number) && !existingNumbers.Contains(number))
            {
                existingNumbers.Add(number);
                numbersProperty.arraySize++;
                numbersProperty.GetArrayElementAtIndex(numbersProperty.arraySize - 1).intValue = number;
            }
        }
    }

}
