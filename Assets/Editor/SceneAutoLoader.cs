#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneAutoLoader
{
    private static string _previousScenePath;
    
    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Проверяем, сохранена ли текущая сцена
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                // Если сцена не сохранена, спрашиваем пользователя, хочет ли он её сохранить
                if (EditorUtility.DisplayDialog(
                        "Сцена не сохранена",
                        "Хотите сохранить изменения в текущей сцене перед переходом в Play Mode?",
                        "Сохранить",
                        "Не сохранять"))
                {
                    // Сохраняем сцену
                    EditorSceneManager.SaveOpenScenes();
                }
            }

            // Переключаемся на сцену с buildIndex = 0, если текущая сцена не является стартовой
            Scene currentScene = SceneManager.GetActiveScene();
            
            if (currentScene.buildIndex != 0)
            {
                string scenePath = GetScenePathByBuildIndex(0);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
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