using System.Collections.Generic;
using UnityEngine;
using static LevelChangeObserver;

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
    public int health;
    public float coins;
    public bool isAlive;
    public int sceneToLoad;
    public bool testSuccess;
    public int spawnPoint; // Новое поле для хранения позиции объекта
    public List<bool> stateSpawnPoints;
    public List<string> alreadyExistWeapons;
    public OneLeson[] progress;
    public int chargeEnergy;
    public int IDmaxRegionAvaliable;
}
