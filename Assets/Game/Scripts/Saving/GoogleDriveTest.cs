using UnityEngine;
using UnityEngine.Networking;
using RPG.Core;
using System.Collections;
using System.Text;

public class GameAPI : MonoBehaviour
{
    public DataPlayer dataPlayer;
    public SceneLoader sceneLoader;
    public string playerID = "111";

    public void Awake()
    {
        StartCoroutine(GetGameData());
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

    IEnumerator SaveGameData(PlayerData playerData)
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
