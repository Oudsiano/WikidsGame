using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
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

        private bool _isConfigReceived;
        private string _receivedJson;

        public bool IsConfigReceived => _isConfigReceived;
        public string ReceivedJson => _receivedJson;

        public void Construct(DataPlayer dataPlayer)
        {
            gameObject.name = "JavaScriptHook";
            _dataPlayer = dataPlayer;
        }

        private void Awake()
        {
            gameObject.name = "JavaScriptHook";

#if UNITY_EDITOR
            // В редакторе выставляем флаг сразу, чтобы тестировать без браузера
            _isConfigReceived = true;
            Debug.Log("🧪 UNITY_EDITOR: _isConfigReceived установлен в true для локального теста");
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SendMessage("UpdateConfigJson",
                    JsonUtility.ToJson(new ConfigData(100, 100, true,
                        Constants.Scenes.FirstBattleScene, 2, true)));
            }
        }

        public void UpdateConfigJson(string json)
        {
            _isConfigReceived = true;

            ConfigData configData = JsonUtility.FromJson<ConfigData>(json);
            DisplayData(configData);

            _dataPlayer.PlayerData.id = configData.id;
            _dataPlayer.PlayerData.health = configData.health;
            _dataPlayer.PlayerData.isAlive = configData.isAlive;
            _dataPlayer.PlayerData.sceneToLoad = configData.sceneToLoad;
            _dataPlayer.PlayerData.sceneNameToLoad = configData.SceneNameToLoad;
            _dataPlayer.PlayerData.testSuccess = configData.testSuccess;

            Debug.Log("✅ UpdateConfigJson вызван с данными: " + json);
        }

        private void DisplayData(ConfigData data)
        {
            string displayString =
                $"ID: {data.id}\nHealth: {data.health}\nIsAlive: {data.health}\nSceneToLoad: {data.sceneToLoad} \ntestSuccess: {data.testSuccess}";

            _dataText.text = displayString;
        }
    }
}