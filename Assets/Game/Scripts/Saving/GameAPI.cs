using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AINavigation;
using Core;
using Core.Player;
using Core.Quests;
using Data;
using DialogueEditor;
using Newtonsoft.Json;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Saving
{
    public class GameAPI : MonoBehaviour
    {
        private MainPlayer _player;
        private SaveGame _saveGame;
        private FastTestsManager _fastTestsManager;
        private PlayerController _playerController;
        private WeaponArmorManager _weaponArmorManager;
        private QuestManager _questManager;
        private DataPlayer _dataPlayer;

        public string playerID;
        public TMP_Text textForOtl;
        public bool idUpdate = false;
        public bool gameSave = false;
        public bool gameGet = false;
        private bool GameLoaded = false;
        private bool _gameLoad = false;

        public bool TestSuccessKey = false; //Ключ нужен отдельно, потому,
        //что диалог его сбрасывает. И нам надо хранить его запределами диалогов.

        private bool needMakeSaveInNextUpdate = false;

        public bool GameLoad => _gameLoad;
        
        public void Construct(MainPlayer player, DataPlayer dataPlayer, SaveGame saveGame,
            FastTestsManager fastTestsManager, PlayerController playerController,
            WeaponArmorManager weaponArmorManager, QuestManager questManager)
        {
            Debug.Log("GameAPI constructed");
            _player = player;
            _dataPlayer = dataPlayer;
            _saveGame = saveGame;
            _fastTestsManager = fastTestsManager;
            _playerController = playerController;
            _weaponArmorManager = weaponArmorManager;
            _questManager = questManager;

            UpdateID();

            TryInitDataServer();
            SetupLoad();
            textForOtl.text =
                $"ID установлен: {idUpdate}\nИгра сохранена: {gameSave}\nИгра загружена на сервер: {gameGet}";
        }

        public void FixedUpdate()
        {
            textForOtl.text =
                $"ID установлен: {idUpdate}\nИгра сохранена: {gameSave}\nИгра загружена на сервер: {gameGet}";
        }

        private void Update()
        {
            if (GameLoaded == false)
            {
                return;
            }

            if (needMakeSaveInNextUpdate)
            {
                needMakeSaveInNextUpdate = false;
                StartCoroutine(SaveGameData());
                gameSave = true;
            }
        }

        public void TryInitDataServer() // TODO not used code
        {
            //StartCoroutine(TryInit());
            gameSave = true;
        }

        public void SaveUpdater()
        {
            needMakeSaveInNextUpdate = true;
        }

        public void UpdateDataTest(int IDLesson, ConversationStarter _currentConversation)
        {
            StartCoroutine(GetGameDataTest(IDLesson, _currentConversation));
        }

        public void IsTestCompleted(int testId, Action<bool> callback) // TODO need to return bool
        {
            StartCoroutine(CheckTestCompletionOnServer(testId, callback));
        }

        private void SetupLoad()
        {
            StartCoroutine(FirstGetGameData());
        }

        private IEnumerator FirstGetGameData()
        {
            Debug.Log("Requesting game data for playerID: " + playerID);
            UnityWebRequest
                request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID); // TODO can be cached
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                SL_objs sl_obj = new SL_objs(json);
                sl_obj.Load(ref playerData.startedQuests, "startedQuests"); // TODO can be cached
                
                _dataPlayer.PlayerData = playerData;

                // Инициализация списка пройденных квестов
                if (_dataPlayer.PlayerData.completedQuests == null)
                {
                    _dataPlayer.PlayerData.completedQuests = new List<string>();
                }

                Debug.Log("Data downloaded successfully");
                _saveGame.MakeLoad();

                _fastTestsManager.GenAvaliableTests();
                gameGet = true;
                GameLoaded = true;
            }
            else
            {
                ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);

                if ((errorResponse.message == "Game data not found") ||
                    (errorResponse.message == "User not found")) // TODO can be cached
                {
                    _playerController.GetFighter()
                        .EquipWeapon(
                            _weaponArmorManager.TryGetWeaponByName("Sword")); // TODO can be cached
                    StartCoroutine(SaveGameData());

                    _fastTestsManager.GenAvaliableTests();
                    GameLoaded = true;
                }
                else
                {
                    Debug.LogError("Error downloading data: " + request.error);
                }
            }
        }

        private IEnumerator GetGameDataTest(int IDLesson, ConversationStarter _currentConversation)
        {
            TestSuccessKey = false;

            UnityWebRequest
                request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID); // TODO can be cached
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

                SL_objs sl_obj = new SL_objs(json);
                sl_obj.Load(ref playerData.startedQuests, "startedQuests"); // TODO can be cached
                _dataPlayer.PlayerData = playerData;

                int countSuccessAnswer = 0;

                if (_dataPlayer.PlayerData.progress != null)
                {
                    foreach (OneLeson item in _dataPlayer.PlayerData.progress)
                    {
                        if (item != null && item.id == IDLesson)
                        {
                            foreach (OneTestQuestion item2 in item.tests)
                            {
                                if (item2.completed)
                                {
                                    _questManager.CompleteQuest(item2.id.ToString());
                                    countSuccessAnswer++;
                                }
                            }
                        }
                    }
                }

                int countSuccessAnswers = countSuccessAnswer;
                _player.ChangeCountEnergy(countSuccessAnswers);
                TestSuccessKey = countSuccessAnswers > 0;
                ConversationManager.Instance.SetBool("TestSuccess", TestSuccessKey);
                //ConversationManager.Instance.SetBool("LoadedData", true);

                if (_currentConversation != null)
                {
                    _currentConversation.waitStartSecondDialog = true;
                }

                Debug.Log("Успешные ответы: " + countSuccessAnswers);
                Debug.Log("Data downloaded successfully");

                _fastTestsManager.GenAvaliableTests(); // TODO rename
            }
            else
            {
                Debug.LogError("Error downloading data: " + request.error);
            }
        }

        private IEnumerator CheckTestCompletionOnServer(int testId, Action<bool> callback)
        {
            UnityWebRequest
                request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID); // TODO can be cached
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                bool _calback = false;
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                _dataPlayer.PlayerData = playerData;

                if (_dataPlayer.PlayerData.progress != null)
                {
                    foreach (var lesson in _dataPlayer.PlayerData.progress)
                    {
                        foreach (var test in lesson.tests)
                        {
                            if (test.completed)
                                _questManager.CompleteQuest(test.id.ToString());

                            if (test.id == testId)
                            {
                                Debug.Log("game api work" + test.completed + test.id);
                                _calback = test.completed;

                                //yield break; Убрал, чтобы не прерывать обсчет всего массива // TODO not used code
                            }
                        }
                    }
                }

                _fastTestsManager.GenAvaliableTests(); // TODO rename
                // Если тест не найден, вызываем колбэк с false
                callback(_calback);
            }
            else
            {
                Debug.LogError("Error downloading data: " + request.error);
                callback(false);
            }
        }

        private void UpdateID() // TODO Duplicate
        {
            playerID = _dataPlayer.PlayerData.id.ToString();
            idUpdate = true;
        }

        private IEnumerator SaveGameData()
        {
            //string json = JsonUtility.ToJson(_dataPlayer.playerData);
            string json = JsonConvert.SerializeObject(_dataPlayer.PlayerData);
            Debug.Log("JSON to send: " + json);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            UnityWebRequest
                request = new UnityWebRequest("https://wikids.ru/api/v1/game", "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(json );
                _gameLoad = true;
                Debug.Log("Data saved successfully");
            }
            else
            {
                Debug.LogError("Error saving data: " + request.error);
            }
        }
    }
}