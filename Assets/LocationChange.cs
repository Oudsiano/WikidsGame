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
        dataPlayer.SetSceneToLoad(LevelChangeObserver.allScenes.battle1);
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");


    }
    public void ChangeButtonBattleTwo()
    {
        dataPlayer.SetSceneToLoad(LevelChangeObserver.allScenes.battleScene2);
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");


    }

    public void ChangeButtonTown()
    {
        dataPlayer.SetSceneToLoad(LevelChangeObserver.allScenes.town1);
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");

    }
    public void ChangeButtonLibrary()
    {
        dataPlayer.SetSceneToLoad(LevelChangeObserver.allScenes.town2);
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");

    }
    public void ChangeButtonHoll()
    {
        dataPlayer.SetSceneToLoad(LevelChangeObserver.allScenes.holl);
        Loading.gameObject.SetActive(true);
        // Устанавливаем значение sceneToLoad в DataPlayer и вызываем метод UpdateData через 2 секунды
        Debug.Log("Изменили значение scene to load в playerdata из LocationChange");
        gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");

    }

    private void LoadSceneAfterDelay()
    {
        sceneLoader.LoadScene((LevelChangeObserver.allScenes)dataPlayer.playerData.sceneToLoad);
        Loading.gameObject.SetActive(false);

    }
}
