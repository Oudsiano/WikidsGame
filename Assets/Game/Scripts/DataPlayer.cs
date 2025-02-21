using System.Collections.Generic;
using Core.Quests;
using UnityEngine;
using static SceneManagement.LevelChangeObserver;

public class DataPlayer : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();

    // Метод для установки номера локации для загрузки
    public void SetSceneToLoad(allScenes sceneId)
    {
        playerData.sceneToLoad = (int)sceneId;
    }

    // Метод для сохранения позиции объекта
    public void SavePlayerPosition(int spawn)
    {
        playerData.spawnPoint = spawn;
    }

    // Метод для загрузки сохраненной позиции объекта
    public int LoadPlayerPosition()
    {
        return playerData.spawnPoint;
    }

    // Вспомогательный метод для изменения размера массива
    private bool[] ResizeArray(bool[] array, int newSize)
    {
        bool[] newArray = new bool[newSize];
        for (int i = 0; i < Mathf.Min(array.Length, newSize); i++)
        {
            newArray[i] = array[i];
        }
        return newArray;
    }

    public bool isTestComplete(int idTest)
    {
        if (playerData.progress == null || playerData.progress.Length == 0)
            return false;

        foreach (OneLeson lesson in playerData.progress)
            if (lesson.tests != null)
                foreach (OneTestQuestion test in lesson.tests)
                    if (test.id == idTest)
                        return test.completed;

        return false;
    }
}

[System.Serializable]
public class OneLeson
{
    public int id;
    public string title;
    public bool completed;
    public OneTestQuestion[] tests;
}


[System.Serializable]
public class OneTestQuestion
{
    public int id;
    public string title;
    public bool completed;
}


[System.Serializable]
public class PlayerData
{    
    public int id;
    public string playerName;
    public int health;
    public double coins;
    public bool isAlive;
    public int sceneToLoad;
    public bool testSuccess;
    public int spawnPoint; // Новое поле для хранения позиции объекта
    public List<bool> stateSpawnPoints;
    public List<string> alreadyExistWeapons;
    public string weaponToLoad = "Sword";
    public int armorIdToload;
    public OneLeson[] progress;
    public int chargeEnergy;
    public List<int> FinishedRegionsIDs;

    public string[] containsBug;
    public List<OneItemForSave> containsBug2;
    public List<string> completedQuests;
    public Dictionary<string, OneQuestData> startedQuests = new Dictionary<string, OneQuestData>();

    public List<int> wasSuccessTests;

    public int helpIndex;

    public float soundVol=1;
    public float musicVol=1;
    public bool soundOn=true;
    public bool musicOn=true;
    public int arrowsCount = 0;

}
