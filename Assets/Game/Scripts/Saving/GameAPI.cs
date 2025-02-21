using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core.Player;
using DialogueEditor;
using Newtonsoft.Json;
using RPG.Core;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Saving
{
    public class GameAPI : MonoBehaviour
    {
        private DataPlayer dataPlayer;
        public SceneLoader sceneLoader;
        public string playerID;
        public TMP_Text textForOtl;
        public bool idUpdate = false;
        public bool gameSave = false;
        public bool gameGet = false;
        private bool GameLoaded = false;

        public bool
            TestSuccessKey =
                false; //Ключ нужен отдельно, потому, что диалог его сбрасывает. И нам надо хранить его запределами диалогов.

        private bool needMakeSaveInNextUpdate = false;

        public void Start() // TODO construct
        {
            dataPlayer = IGame.Instance.dataPlayer;

            UpdateID();

            //TryInitDataServer();
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
            UnityWebRequest
                request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID); // TODO can be cached
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                SL_objs sl_obj = new SL_objs(json);
                sl_obj.Load(ref playerData.startedQuests, "startedQuests"); // TODO can be cached

                dataPlayer.PlayerData = playerData;

                // Инициализация списка пройденных квестов
                if (dataPlayer.PlayerData.completedQuests == null)
                {
                    dataPlayer.PlayerData.completedQuests = new List<string>();
                }

                Debug.Log("Data downloaded successfully");
                IGame.Instance.saveGame.MakeLoad();

                IGame.Instance.FastTestsManager.GenAvaliableTests();
                gameGet = true;
                GameLoaded = true;
            }
            else
            {
                ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);

                if ((errorResponse.message == "Game data not found") ||
                    (errorResponse.message == "User not found")) // TODO can be cached
                {
                    IGame.Instance.playerController.GetFighter()
                        .EquipWeapon(
                            IGame.Instance.WeaponArmorManager.TryGetWeaponByName("Sword")); // TODO can be cached
                    StartCoroutine(SaveGameData());

                    IGame.Instance.FastTestsManager.GenAvaliableTests();
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
                dataPlayer.PlayerData = playerData;

                int countSuccessAnswer = 0;

                if (IGame.Instance.dataPlayer.PlayerData.progress != null)
                {
                    foreach (OneLeson item in IGame.Instance.dataPlayer.PlayerData.progress)
                    {
                        if (item != null && item.id == IDLesson)
                        {
                            foreach (OneTestQuestion item2 in item.tests)
                            {
                                if (item2.completed)
                                {
                                    IGame.Instance.QuestManager.QuestFinished(item2.id.ToString());
                                    countSuccessAnswer++;
                                }
                            }
                        }
                    }
                }

                int countSuccessAnswers = countSuccessAnswer;
                MainPlayer.Instance.ChangeCountEnergy(countSuccessAnswers);
                TestSuccessKey = countSuccessAnswers > 0;
                ConversationManager.Instance.SetBool("TestSuccess", TestSuccessKey);
                //ConversationManager.Instance.SetBool("LoadedData", true);

                if (_currentConversation != null)
                {
                    _currentConversation.waitStartSecondDialog = true;
                }

                Debug.Log("Успешные ответы: " + countSuccessAnswers);
                Debug.Log("Data downloaded successfully");

                IGame.Instance.FastTestsManager.GenAvaliableTests(); // TODO rename
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
                dataPlayer.PlayerData = playerData;

                if (dataPlayer.PlayerData.progress != null)
                {
                    foreach (var lesson in dataPlayer.PlayerData.progress)
                    {
                        foreach (var test in lesson.tests)
                        {
                            if (test.completed)
                                IGame.Instance.QuestManager.QuestFinished(test.id.ToString());

                            if (test.id == testId)
                            {
                                Debug.Log("game api work");
                                _calback = test.completed;

                                //yield break; Убрал, чтобы не прерывать обсчет всего массива // TODO not used code
                            }
                        }
                    }
                }

                IGame.Instance.FastTestsManager.GenAvaliableTests(); // TODO rename
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
            playerID = dataPlayer.PlayerData.id.ToString();
            idUpdate = true;
        }

        private IEnumerator SaveGameData()
        {
            //string json = JsonUtility.ToJson(IGame.Instance.dataPLayer.playerData);
            string json = JsonConvert.SerializeObject(IGame.Instance.dataPlayer.PlayerData);
            Debug.Log("JSON to send: " + json);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            UnityWebRequest
                request = new UnityWebRequest("https://wikids.ru/api/v1/game", "POST"); // TODO can be cached
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data saved successfully");
            }
            else
            {
                Debug.LogError("Error saving data: " + request.error);
            }
        }
    }
}