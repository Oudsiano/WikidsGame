using System.Collections.Generic;
using Core.Quests.Data;
using Saving;

[System.Serializable]
public class PlayerData // TODO need restruct
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

    public float soundVol = 1;
    public float musicVol = 1;
    public bool soundOn = true;
    public bool musicOn = true;
    public int arrowsCount = 0;
}