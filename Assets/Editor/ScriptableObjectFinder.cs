/*using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using FarrokhGames.Inventory.Examples;

[InitializeOnLoad]
public class ScriptableObjectFinder
{


    static ScriptableObjectFinder()
    {

        FindAllScriptableObjects();
    }

    public static void FindAllScriptableObjects()
    {
        QuestManager questManager = new QuestManager();
        string prefabPath = "Assets/Game/Prefabs/Game/core/Core.prefab";
        //string[] QuestManagers = AssetDatabase.FindAssets($"t:GameObject Core");

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (prefabInstance.GetComponent<QuestManager>())
            {
                questManager = prefabInstance.GetComponent<QuestManager>();
            }



            if (questManager == null) return;

            questManager.allQuestsItems.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ItemDefinition");
            List<ItemDefinition> scriptableObjects = new List<ItemDefinition>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemDefinition so = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                if (so != null)
                {
                    scriptableObjects.Add(so);
                }
            }

            foreach (var so in scriptableObjects)
            {
                if (so.Type == ItemType.QuestItem)
                    questManager.allQuestsItems.Add(so);

            }


            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            GameObject.DestroyImmediate(prefabInstance);

            Debug.Log($"Found {questManager.allQuestsItems.Count} ItemDefinition quests:");
        }
        else
        {
            Debug.LogWarning("Не найден QuestManagers. в целом не страшно, но надо как-то тогда самому закинуть айтемы сюда");
            return;
        }
    }
}

public class ScriptableObjectPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        ScriptableObjectFinder.FindAllScriptableObjects();
    }
}*/