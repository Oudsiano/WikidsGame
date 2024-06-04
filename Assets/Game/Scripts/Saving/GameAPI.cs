using UnityEngine;
using UnityEngine.Networking;
using RPG.Core;
using System.Collections;
using System.Text;
using TMPro;
using DialogueEditor;
using System;

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
            dataPlayer.playerData = playerData;
            Debug.Log("Data downloaded successfully");

            IGame.Instance.saveGame.MakeLoad();

            gameGet = true;
            //sceneLoader.TryChangeLevel((LevelChangeObserver.allScenes)dataPlayer.playerData.sceneToLoad);
            GameLoaded = true;
        }
        else
        {
            ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);

            if (errorResponse.message == "Game data not found")
            {
                IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName("Sword"));
                SaveUpdater();

                GameLoaded = true;
            }
            else

                Debug.LogError("Error downloading data: " + request.error);

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
                                countSuccessAnswer++;
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


            Debug.Log("?????????? ??????? " + countSuccessAnswers);

            Debug.Log("Data downloaded successfully");
        }
        else
        {
            Debug.LogError("Error downloading data: " + request.error);
        }
    }

    public bool IsTestCompleted(int testId)
    {
        if (dataPlayer.playerData.progress != null)
        {
            foreach (var lesson in dataPlayer.playerData.progress)
            {
                foreach (var test in lesson.tests)
                {
                    if (test.id == testId)
                    {
                        Debug.Log("game api work");
                        return test.completed;
                    }
                }
            }
        }
        Debug.Log("game api work");
        return false;
    }

    public IEnumerator SaveGameData()
    {
        string json = JsonUtility.ToJson(IGame.Instance.dataPLayer.playerData);
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
