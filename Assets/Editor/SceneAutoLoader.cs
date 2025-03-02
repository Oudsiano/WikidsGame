#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneAutoLoader
{
    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            
            if (currentScene.buildIndex != 0)
            {
                EditorSceneManager.OpenScene(GetScenePathByBuildIndex(0));
            }
        }
    }
    
    private static string GetScenePathByBuildIndex(int buildIndex)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError($"Сцена с buildIndex = {buildIndex} не найдена в Build Settings.");
            return null;
        }
        return scenePath;
    }
}
#endif