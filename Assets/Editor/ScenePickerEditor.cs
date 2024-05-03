/*
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelChangeObserver), true)]
public class ScenePickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //var picker = target as ScenePicker;

        var SceneList = target as LevelChangeObserver;
        foreach (var item in SceneList.AllScenes)
        {
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(item.fileScene);

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField("scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty("fileScene");
                scenePathProperty.stringValue = newPath;
            }
            serializedObject.ApplyModifiedProperties();
        }


        
    }
}*/