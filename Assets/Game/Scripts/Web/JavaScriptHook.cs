using System;
using System.ComponentModel;
using Data;
using Saving;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Web.Data;

namespace Web
{
    public class JavaScriptHook : MonoBehaviour
    {
        [FormerlySerializedAs("dataText")] [SerializeField]
        private TMP_Text _dataText; // Ссылка на текстовый объект

        [FormerlySerializedAs("dataPlayer")] [SerializeField]
        private DataPlayer _dataPlayer; // Ссылка на экземпляр DataPlayer

        public bool IsHooked { get; private set; } = false;

        public void Construct(DataPlayer dataPlayer)
        {
            gameObject.name = "JavaScriptHook";
            Debug.Log("JavaScriptHook Constructed");
            
            _dataPlayer = dataPlayer;

            if (_dataText == null)
            {
                Debug.LogError("Text object reference is not set!");
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SendMessage("UpdateConfigJson",
                    JsonUtility.ToJson(new ConfigData(100, 100, true, Constants.Scenes.FirstBattleScene, 2, true)));
            }
        }
        
        public void UpdateConfigJson(string json)
        {
            Debug.Log(".PlayerData.id right now => " + _dataPlayer.PlayerData.id);
            ConfigData configData = JsonUtility.FromJson<ConfigData>(json);
            // Здесь вы можете использовать данные configData по вашему усмотрению
            DisplayData(configData); // Вызываем метод отображения данных

            Debug.Log("loadConfigFromHtml");
            Debug.Log("configData.id right now => " + configData.id);

            // Передаем данные в DataPlayer
            _dataPlayer.PlayerData.id = configData.id;
            _dataPlayer.PlayerData.health = configData.health;
            _dataPlayer.PlayerData.isAlive = configData.isAlive;
            _dataPlayer.PlayerData.sceneToLoad = configData.sceneToLoad;
            _dataPlayer.PlayerData.sceneNameToLoad = configData.SceneNameToLoad;
            _dataPlayer.PlayerData.testSuccess = configData.testSuccess;

            Debug.Log("_dataPlayer.PlayerData.id after loadConfigFromHtml => " + _dataPlayer.PlayerData.id);

            Debug.Log("✅ UpdateConfigJson вызван с данными: " + json);
            IsHooked = true;
            
            //TODO Если загрузка не произошла убери комменты и запусти из этого метода загрузку сцены.
            //sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
        }

        // Метод для отображения данных в текстовом объекте
        private void DisplayData(ConfigData data)
        {
            // Формируем строку с данными
            string displayString =
                $"ID: {data.id}\nHealth: {data.health}\nIsAlive: {data.health}\nSceneToLoad: {data.sceneToLoad} \ntestSuccess: {data.testSuccess}";

            // Устанавливаем сформированную строку в текстовый объект
            _dataText.text = displayString;
        }
    }
}