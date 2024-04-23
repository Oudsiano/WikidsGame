using UnityEngine;
using UnityEngine.Networking;
using RPG.Core;
using System.Collections;
using System.Text;
using TMPro;
using DialogueEditor;

public class GameAPI : MonoBehaviour
{
    private DataPlayer dataPlayer;
    public SceneLoader sceneLoader;
    public string playerID;
    public TMP_Text textForOtl;
    public bool idUpdate = false;
    public bool gameSave = false;
    public bool gameGet = false;

    public void Start()
    {
        dataPlayer = IGame.Instance.dataPLayer;

        IDUpdater();
        SaveUpdater();
        UpdateData();
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
    public void SaveUpdater()
    {
        StartCoroutine(SaveGameData(dataPlayer.playerData));
        gameSave = true;
    }
    public void UpdateData()
    {
        StartCoroutine(GetGameData());
        gameGet = true;
        sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
    }

    public void UpdataDataTest()
    {
        StartCoroutine(GetGameDataTest());
    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(GetGameData());
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(SaveGameData(dataPlayer.playerData));
        }
    }

    IEnumerator GetGameData()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://wikids.ru/api/v1/game/" + playerID);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            dataPlayer.playerData = playerData;
            Debug.Log("Data downloaded successfully");
            sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
        }
        else
        {
            Debug.LogError("Error downloading data: " + request.error);
        }
    }

    IEnumerator GetGameDataTest()
    {
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
                    if (item != null)
                        foreach (OneTestQuestion item2 in item.tests)
                        {
                            if (item2.completed)
                                countSuccessAnswer++;
                        }
                }

            }
            int countSuccessAnswers = countSuccessAnswer;
            RPG.Core.MainPlayer.Instance.ChangeCountEnegry(countSuccessAnswers);

            ConversationManager.Instance.SetBool("TestSuccess", countSuccessAnswers > 0);

            Debug.Log("?????????? ??????? " + countSuccessAnswers);

            Debug.Log("Data downloaded successfully");        }
        else
        {
            Debug.LogError("Error downloading data: " + request.error);
        }
    }

    public IEnumerator SaveGameData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData);
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
