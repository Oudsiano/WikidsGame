using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core.Player;
using DialogueEditor;
using Newtonsoft.Json;
using RPG.Core;
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

        public bool TestSuccessKey = false; //Ключ нужен отдельно, потому, что диалог его сбрасывает. И нам надо хранить его запределами диалогов.

        private bool needMakeSaveInNextUpdate = false;

        public void Start()
        {
            dataPlayer = IGame.Instance.dataPlayer;

            IDUpdater();

            //TryInitDataServer();
            FirstLoad();
            textForOtl.text = $"ID установлен: {idUpdate}\nИгра сохранена: {gameSave}\nИгра загружена на сервер: {gameGet}";
        }
        public void FixedUpdate()
        {
            textForOtl.text = $"ID установлен: {idUpdate}\nИгра сохранена: {gameSave}\nИгра загружена на сервер: {gameGet}";
        }

        public void IDUpdater()
        {
            playerID = dataPlayer.playerData.id.ToString();
            idUpdate = true;
        }
        public void TryInitDataServer()
        {////
            //StartCoroutine(TryInit());
            gameSave = true;
        }
        public void SaveUpdater()
        {
            needMakeSaveInNextUpdate = true;
        }

        public void FirstLoad()
        {
            StartCoroutine(FirstGetGameData());
        }

        public void UpdataDataTest(int IDLesson, ConversationStarter _currentConversation)
        {
            StartCoroutine(GetGameDataTest(IDLesson, _currentConversation));
        }

        private void Update()
        {
            if (!GameLoaded) return;
            if (needMakeSaveInNextUpdate)
            {
                needMakeSaveInNextUpdate = false;
                StartCoroutine(SaveGameData());
                gameSave = true;
            }
        }

        IEnumerator FirstGetGameData()
        {
            UnityWebRequest request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                SL_objs sl_obj = new SL_objs(json);
                sl_obj.load(ref playerData.startedQuests, "startedQuests");

                dataPlayer.playerData = playerData;

                // Инициализация списка пройденных квестов
                if (dataPlayer.playerData.completedQuests == null)
                {
                    dataPlayer.playerData.completedQuests = new List<string>();
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

                if ((errorResponse.message == "Game data not found") || (errorResponse.message == "User not found"))
                {
                    IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName("Sword"));
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


        IEnumerator GetGameDataTest(int IDLesson, ConversationStarter _currentConversation)
        {
            TestSuccessKey = false;

            UnityWebRequest request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

                SL_objs sl_obj = new SL_objs(json);
                sl_obj.load(ref playerData.startedQuests, "startedQuests");
                dataPlayer.playerData = playerData;

                int countSuccessAnswer = 0;

                if (IGame.Instance.dataPlayer.playerData.progress != null)
                {
                    foreach (OneLeson item in IGame.Instance.dataPlayer.playerData.progress)
                    {
                        if (item != null && item.id == IDLesson)
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
                int countSuccessAnswers = countSuccessAnswer;
                MainPlayer.Instance.ChangeCountEnergy(countSuccessAnswers);
                TestSuccessKey = countSuccessAnswers > 0;
                ConversationManager.Instance.SetBool("TestSuccess", TestSuccessKey);
                //ConversationManager.Instance.SetBool("LoadedData", true);
                if (_currentConversation != null)
                    _currentConversation.waitStartSecondDialog = true;


                Debug.Log("Успешные ответы: " + countSuccessAnswers);

                Debug.Log("Data downloaded successfully");

                IGame.Instance.FastTestsManager.GenAvaliableTests();
            }
            else
            {
                Debug.LogError("Error downloading data: " + request.error);
            }
        }

        public void IsTestCompleted(int testId, Action<bool> callback)
        {
            StartCoroutine(CheckTestCompletionOnServer(testId, callback));
        }

        IEnumerator CheckTestCompletionOnServer(int testId, Action<bool> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                bool _calback = false;
                string json = request.downloadHandler.text;
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                dataPlayer.playerData = playerData;

                if (dataPlayer.playerData.progress != null)
                {
                    foreach (var lesson in dataPlayer.playerData.progress)
                    {
                        foreach (var test in lesson.tests)
                        {
                            if (test.completed)
                                IGame.Instance.QuestManager.QuestFinished(test.id.ToString());

                            if (test.id == testId)
                            {
                                Debug.Log("game api work");
                                // Вызываем колбэк с результатом
                                _calback = test.completed;

                                //yield break; Убрал, чтобы не прерывать обсчет всего массива
                            }
                        }
                    }
                }


                IGame.Instance.FastTestsManager.GenAvaliableTests();
                // Если тест не найден, вызываем колбэк с false
                callback(_calback);
            }
            else
            {
                Debug.LogError("Error downloading data: " + request.error);
                // В случае ошибки вызываем колбэк с false
                callback(false);
            }
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!
        //IT IS PRIVATE. USE IGame.Instance.gameAPI.SaveUpdater(); for save game
        //!!!!!!!!!!!!!!!!!!!!!!!!!!
        private IEnumerator SaveGameData() 
        {
            //string json = JsonUtility.ToJson(IGame.Instance.dataPLayer.playerData);
            string json = JsonConvert.SerializeObject(IGame.Instance.dataPlayer.playerData);
            Debug.Log("JSON to send: " + json);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest("https://wikids.ru/api/v1/game", "POST");
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
