using UnityEngine;
using TMPro;
using SceneManagement;
using UnityEngine.Serialization;
using Web.Data;

public class JavaScriptHook : MonoBehaviour
{
    public SceneLoader sceneLoader; // TODO rename
    
    [FormerlySerializedAs("dataText")][SerializeField] private TMP_Text _dataText; // Ссылка на текстовый объект
    [FormerlySerializedAs("dataPlayer")][SerializeField] private DataPlayer _dataPlayer; // Ссылка на экземпляр DataPlayer

    private void Start() // TODO construct
    {
        if (_dataText == null)
        {
            Debug.LogError("Text object reference is not set!");
        }
    }
  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SendMessage("UpdateConfigJson", JsonUtility.ToJson(new ConfigData(100, 100, true, 2,true)));
        }
    }
    
    public void UpdateConfigJson(string json)
    {
        ConfigData configData = JsonUtility.FromJson<ConfigData>(json);
        // Здесь вы можете использовать данные configData по вашему усмотрению
        DisplayData(configData); // Вызываем метод отображения данных

        Debug.Log("loadConfigFromHtml");

        // Передаем данные в DataPlayer
        _dataPlayer.PlayerData.id = configData.id;
        _dataPlayer.PlayerData.health = configData.health;
        _dataPlayer.PlayerData.isAlive = configData.isAlive;
        _dataPlayer.PlayerData.sceneToLoad = configData.sceneToLoad;
        _dataPlayer.PlayerData.testSuccess = configData.testSuccess;
        //TODO Если загрузка не произошла убери комменты и запусти из этого метода загрузку сцены.
        //sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
    }

    // Метод для отображения данных в текстовом объекте
    private void DisplayData(ConfigData data)
    {
        // Формируем строку с данными
        string displayString = $"ID: {data.id}\nHealth: {data.health}\nIsAlive: {data.health}\nSceneToLoad: {data.sceneToLoad} \ntestSuccess: {data.testSuccess}";

        // Устанавливаем сформированную строку в текстовый объект
        _dataText.text = displayString;
    }
}
