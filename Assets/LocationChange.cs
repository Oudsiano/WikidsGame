using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using TMPro;

public class LocationChange : MonoBehaviour
{
    [SerializeField] public DataPlayer dataPlayer;
    [SerializeField] private GameAPI gameAPI;
    public SceneLoader sceneLoader;
    public TMP_Text Loading;

    private void Start()
    {
        // Находим объекты DataPlayer и GameAPI в сцене
        dataPlayer = FindObjectOfType<DataPlayer>();
        gameAPI = FindObjectOfType<GameAPI>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void ChangeButtonBattle()
    {
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        dataPlayer.SetSceneToLoad(5);
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 5f);

    }

    public void ChangeButtonTown()
    {
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        dataPlayer.SetSceneToLoad(6);
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 5f);
    }

    private void LoadSceneAfterDelay()
    {
        sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
        Loading.gameObject.SetActive(false);

    }
}
