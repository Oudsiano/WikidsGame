using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Core;

public class JavaScriptHook : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public TMP_Text dataText; // Ссылка на текстовый объект
    public DataPlayer dataPlayer; // Ссылка на экземпляр DataPlayer

    private void Start()
    {
        // Проверяем, что ссылка на текстовый объект установлена
        if (dataText == null)
        {
            Debug.LogError("Text object reference is not set!");
        }
    }

    public void UpdateConfigJson(string json)
    {
        ConfigData configData = JsonUtility.FromJson<ConfigData>(json);
        // Здесь вы можете использовать данные configData по вашему усмотрению
        DisplayData(configData); // Вызываем метод отображения данных

        // Передаем данные в DataPlayer
        dataPlayer.playerData.id = configData.id;
        dataPlayer.playerData.health = configData.health;
        dataPlayer.playerData.isAlive = configData.isAlive;
        dataPlayer.playerData.sceneToLoad = configData.sceneToLoad;
        dataPlayer.playerData.testSuccess = configData.testSuccess;
        //TODO Если загрузка не произошла убери комменты и запусти из этого метода загрузку сцены.
        //sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);

    }

    // Метод для отображения данных в текстовом объекте
    private void DisplayData(ConfigData data)
    {
        // Формируем строку с данными
        string displayString = $"ID: {data.id}\nHealth: {data.health}\nIsAlive: {data.health}\nSceneToLoad: {data.sceneToLoad} \ntestSuccess: {data.testSuccess}";

        // Устанавливаем сформированную строку в текстовый объект
        dataText.text = displayString;
    }

  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SendMessage("UpdateConfigJson", JsonUtility.ToJson(new ConfigData(100, 100, true, 2,true)));
        }
    }

    [System.Serializable]
    public class ConfigData
    {
        public int id;
        public int health;
        public bool isAlive;
        public int sceneToLoad;
        public bool testSuccess;
        public ConfigData(int id, int health, bool isAlive, int sceneToLoad,bool testSuccess)
        {
            this.id = id;
            this.health = health;
            this.isAlive = isAlive;
            this.sceneToLoad = sceneToLoad;
            this.testSuccess = testSuccess;
        }
    }
}
