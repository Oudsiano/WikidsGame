using UnityEngine;
using UnityEngine.Networking;
using RPG.Core;
using System.Collections;
using System.Text;
using TMPro;
using DialogueEditor;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;



public class SL_objs
{
    public Dictionary<string, object> objs;
    public SL_objs(string _load)
    {
        objs = JsonConvert.DeserializeObject<Dictionary<string, object>>(_load);
    }

    private void unpars<T>(ref T _ob, string i)
    {


        if (objs.ContainsKey(i))
        {
            if (_ob is bool)
            {
                _ob = JsonConvert.DeserializeObject<T>(objs[i].ToString().ToLower());
            }
            else if (_ob is float)
            {
                _ob = JsonConvert.DeserializeObject<T>(objs[i].ToString().Replace(',', '.'));
            }
            else
            {
                object obj = objs[i];
                if (obj != null)
                {
                    string str1 = obj.ToString();
                    if (str1.Length>2)
                    _ob = JsonConvert.DeserializeObject<T>(str1);
                }
        }
        }
        else
            Debug.Log("Загрузка пытается получить больше данных чем есть в файле.");
    }

    internal void load(ref List<string> _ob, string v)
    {
        unpars<List<string>>(ref _ob, v);
    }

    internal void load(ref double _ob, string v)
    {
        unpars<double>(ref _ob, v);
    }
    internal void load(ref int _ob, string v)
    {
        unpars<int>(ref _ob, v);
    }
    internal void load(ref long _ob, string v)
    {
        unpars<long>(ref _ob, v);
    }
    internal void load(ref string _ob, string v)
    {
        unpars<string>(ref _ob, v);
    }
    internal void load(ref bool _ob, string v)
    {
        unpars<bool>(ref _ob, v);
    }

    internal void load(ref Dictionary<string, string> _ob, string v)
    {
        unpars<Dictionary<string, string>>(ref _ob, v);
    }

    internal void load(ref Dictionary<string, int> _ob, string v)
    {
        unpars<Dictionary<string, int>>(ref _ob, v);
    }
    
    internal void load(ref Dictionary<int, bool> _ob, string v)
    {
        unpars<Dictionary<int, bool>>(ref _ob, v);
    }
    internal void load(ref Dictionary<string, OneQuestData> _ob, string v)
    {
        unpars<Dictionary<string, OneQuestData>>(ref _ob, v);
    }
}


[Serializable]
public class ErrorResponse
{
    public string name;
    public string message;
    public int code;
    public int status;
    public string type;
}

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
        dataPlayer = IGame.Instance.dataPLayer;

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

            if (IGame.Instance.dataPLayer.playerData.progress != null)
            {
                foreach (OneLeson item in IGame.Instance.dataPLayer.playerData.progress)
                {
                    if (item != null && item.id == IDLesson)
                        foreach (OneTestQuestion item2 in item.tests)
                        {

                            if (item2.completed)
                            {
                                IGame.Instance.QuestManager.questFinished(item2.id.ToString());
                                countSuccessAnswer++;
                            }
                        }


                }

            }
            int countSuccessAnswers = countSuccessAnswer;
            RPG.Core.MainPlayer.Instance.ChangeCountEnegry(countSuccessAnswers);
            TestSuccessKey = countSuccessAnswers > 0;
            ConversationManager.Instance.SetBool("TestSuccess", TestSuccessKey);
            //ConversationManager.Instance.SetBool("LoadedData", true);
            if (_currentConversation != null)
                _currentConversation.waitStartSecondDialog = true;


            Debug.Log("Успешные ответы: " + countSuccessAnswers);

            Debug.Log("Data downloaded successfully");
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
                        IGame.Instance.QuestManager.questFinished(test.id.ToString());

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
        string json = JsonConvert.SerializeObject(IGame.Instance.dataPLayer.playerData);
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
