using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static LevelChangeObserver;
using System.Linq;

public class LocationChange : MonoBehaviour
{
    [Serializable]
    public class OneBtnChangeRegion
    {
        public Button Button;
        public allScenes loadedScene;
    }

    [SerializeField]
    public List<OneBtnChangeRegion> regions = new List<OneBtnChangeRegion>();

    [SerializeField] private GameObject Loading;
    [SerializeField] private TMP_Text hoverTextDisplay; // Поле для отображения текста при наведении
    [SerializeField] private MultiLineText multiLineText; // Ссылка на компонент с многострочным текстом

    [SerializeField] private TMP_Text textID;

    public void Awake()
    {
        
        if (multiLineText.hoverTexts.Length != regions.Count)
        {
            Debug.LogError("The number of hover texts does not match the number of regions.");
            return;
        }

        for (int i = 0; i < regions.Count; i++)
        {
            int index = i; // Локальная копия переменной, чтобы избежать проблем с замыканием
            regions[i].Button.onClick.AddListener(() => OnClick(regions[index].loadedScene));

            EventTrigger trigger = regions[i].Button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { OnPointerExit(); });
            trigger.triggers.Add(entryExit);
        }

        setUpMaxRegion(IGame.Instance.dataPLayer.playerData.FinishedRegionsIDs);
        Debug.Log("awake changeLoc");

        if (textID != null)
            textID.text = IGame.Instance.dataPLayer.playerData.id.ToString();
    }

    private void Start()
    {
        Loading.gameObject.SetActive(false);
    }

    public void setUpMaxRegion(List<int> n)
    {
        int findedIndex = -1;
        for (int i = regions.Count-1; i >= 0; i--)
        {
            if (n.Contains((int)regions[i].loadedScene))
                findedIndex = i;
        }

        if ((findedIndex+1)<regions.Count)
        {
            findedIndex++;
        }

        for (int i = 0; i < regions.Count; i++)
        {
            if (i <= findedIndex)
                regions[i].Button.interactable = true;
            else
                regions[i].Button.interactable = false;
        }
    }

    private void OnClick(allScenes sceneId)
    {
        IGame.Instance.dataPLayer.SetSceneToLoad(sceneId);
        Loading.gameObject.SetActive(true);
        IGame.Instance.gameAPI.SaveUpdater();
        //Invoke("LoadSceneAfterDelay", 2f); 
        SceneLoader.Instance.TryChangeLevel((LevelChangeObserver.allScenes)IGame.Instance.dataPLayer.playerData.sceneToLoad);
        AudioManager.instance.PlaySound("ClickButton");
    }
    /*
    private void LoadSceneAfterDelay()
    {
        
    }*/

    private void OnPointerEnter(int index)
    {
        if (index >= 0 && index < multiLineText.hoverTexts.Length)
        {
            hoverTextDisplay.text = multiLineText.hoverTexts[index];
        }
        else
        {
            Debug.LogWarning($"Index {index} is out of range for hover texts.");
        }
    }

    private void OnPointerExit()
    {
        hoverTextDisplay.text = string.Empty;
    }
}
