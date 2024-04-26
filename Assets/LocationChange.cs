using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using static LevelChangeObserver;

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

    //private SceneLoader sceneLoader;

    [SerializeField] private TMP_Text Loading;


    public void Awake()
    {
        foreach (OneBtnChangeRegion item in regions)
        {
            item.Button.onClick.AddListener(() => OnClick(item.loadedScene));
        }

        setUpMaxRegion(IGame.Instance.dataPLayer.playerData.IDmaxRegionAvaliable);
    }

    public void setUpMaxRegion(int n)
    {
        int findedIndex = 0;
        for (int i = 0; i < regions.Count; i++)
        {
            if ((int)regions[i].loadedScene <= n)
            {
                regions[i].Button.interactable = true;

                if (findedIndex < i) findedIndex = i;
            }
                
        }

        if (findedIndex< regions.Count)
            for (int i = findedIndex+1; i < regions.Count; i++)
            {
                regions[i].Button.interactable = false;
            }

    }

    private void OnClick(allScenes sceneId)
    {
        IGame.Instance.dataPLayer.SetSceneToLoad(sceneId);
        Loading.gameObject.SetActive(true);
        IGame.Instance.gameAPI.SaveUpdater();
        Invoke("LoadSceneAfterDelay", 2f);
        AudioManager.instance.Play("ClickButton");
    }

    private void LoadSceneAfterDelay()
    {
        SceneLoader.Instance.LoadScene((LevelChangeObserver.allScenes)IGame.Instance.dataPLayer.playerData.sceneToLoad);
        Loading.gameObject.SetActive(false);

    }
}
