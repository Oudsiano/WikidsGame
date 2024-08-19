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
using System.Text.RegularExpressions;

public class LocationChange : MonoBehaviour
{
    [Serializable]
    public class OneBtnChangeRegion
    {
        public Button Button;
        public allScenes loadedScene;

        public void SetRed()
        {
            Button.GetComponent<Image>().color = new Color32(0xFF, 0x73, 0x5F, 0xFF);
        }
        internal void SetGreen()
        {
            Button.GetComponent<Image>().color = new Color32(0x94, 0xFF, 0x5F, 0xFF);
        }

        internal void SetNormal()
        {
            Button.GetComponent<Image>().color = new Color32(0xff, 0xFF, 0xfF, 0xFF);
        }
    }

    [SerializeField]
    public List<OneBtnChangeRegion> regions = new List<OneBtnChangeRegion>();

    [SerializeField] private GameObject Loading;
    [SerializeField] private TMP_Text hoverTextDisplay; // Поле для отображения текста при наведении
    [SerializeField] private TMP_Text TestsTextDisplay;
    [SerializeField] private MultiLineText multiLineText; // Ссылка на компонент с многострочным текстом

    [SerializeField] private TMP_Text textID;

    private SceneWithTestsID sceneWithTestsID;
    private List<string> ListNeedTests;

    

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


        sceneWithTestsID = FindObjectOfType<SceneWithTestsID>();
        updateColors();
        setUpMaxRegion(IGame.Instance.dataPlayer.playerData.FinishedRegionsIDs);

        if (textID != null)
            textID.text = IGame.Instance.dataPlayer.playerData.id.ToString();

    }

    private void Start()
    {
        Loading.gameObject.SetActive(false);
    }

    public void setUpMaxRegion(List<int> n)
    {
        int findedIndex = -1;
        for (int i = regions.Count - 1; i >= 0; i--)
        {
            if (n.Contains((int)regions[i].loadedScene))
            {
                findedIndex = i;
                break;
            }
        }

        if ((findedIndex + 1) < regions.Count)
        {
            findedIndex++;
        }

        for (int i = 0; i < regions.Count; i++)
        {
            if (i <= findedIndex)
                regions[i].Button.interactable = true;
            else
            {
                regions[i].Button.interactable = false;
                regions[i].SetNormal();
            }

            if (i == findedIndex)
                regions[i].SetNormal();
        }
    }

    private void updateColors()
    {
        // Получаем данные игрока один раз, чтобы не вызывать его многократно в цикле.
        var playerData = IGame.Instance.dataPlayer.playerData.progress;

        foreach (OneBtnChangeRegion region in regions)
        {
            // По умолчанию устанавливаем цвет в зеленый.
            region.SetGreen();
            IGame.Instance.LevelChangeObserver.DictForInfected[region.loadedScene] = false;

            // Находим соответствующую сцену для текущего региона.
            var sceneData = sceneWithTestsID.sceneDataList
                .FirstOrDefault(scene => scene.scene == region.loadedScene);

            if (sceneData != null && playerData != null)
            {
                // Проверяем тесты текущей сцены.
                var incompleteTestFound = sceneData.numbers
                    .Any(testScene => playerData
                        .Any(lesson => lesson.tests
                            .Any(test => test.id == testScene && !test.completed)));

                // Если найден незавершенный тест, устанавливаем цвет в красный.
                if (incompleteTestFound)
                {
                    region.SetRed();
                    IGame.Instance.LevelChangeObserver.DictForInfected[region.loadedScene] = true;
                    continue;
                }
            }
        }
    }

    private void OnClick(allScenes sceneId)
    {
        IGame.Instance.dataPlayer.SetSceneToLoad(sceneId);
        Loading.gameObject.SetActive(true);
        IGame.Instance.gameAPI.SaveUpdater();
        //Invoke("LoadSceneAfterDelay", 2f); 
        SceneLoader.Instance.TryChangeLevel(sceneId);
        AudioManager.instance.PlaySound("ClickButton");
    }
    /*
    private void LoadSceneAfterDelay()
    {
        
    }*/

    private void OnPointerEnter(int index)
    {
        

            String needTests = "";
        ListNeedTests = new List<string>();
        if (index >= 0 && index < regions.Count)
        {

            if (regions[index].Button.interactable==true)
            {
                foreach (SceneData scene in sceneWithTestsID.sceneDataList)
                {
                    if (scene.scene == regions[index].loadedScene)
                    {
                        foreach (int testScene in scene.numbers)
                        {
                            if (IGame.Instance.dataPlayer.playerData.progress != null)
                            {
                                foreach (OneLeson item in IGame.Instance.dataPlayer.playerData.progress)
                                {
                                    foreach (OneTestQuestion item2 in item.tests)
                                    {
                                        if (testScene == item2.id)
                                            if (!item2.completed)
                                            {
                                                ListNeedTests.Add(item2.title);
                                            }
                                    }
                                }

                            }

                        }
                    }
                }

                var grouped = ListNeedTests
                .Select(s => new
                {
                    Original = s,
                    Key = Regex.Replace(s, @" Тест \d+| Итоговый тест \d+", ""),
                    TestType = Regex.Match(s, @"(Тест|Итоговый тест)").Value,
                    TestNumber = int.Parse(Regex.Match(s, @"\d+$").Value)
                })
                .GroupBy(x => new { x.Key, x.TestType })
                .Select(g => new
                {
                    Key = g.Key.Key,
                    TestType = g.Key.TestType,
                    TestNumbers = g.Select(x => x.TestNumber).OrderBy(n => n).ToList()
                });

                foreach (var group in grouped)
                {
                    string tests = string.Join(", ", group.TestNumbers);
                    string testWord = group.TestNumbers.Count > 1 ? "Тесты" : "Тест";
                    if (group.TestType == "Итоговый тест")
                        testWord = group.TestNumbers.Count > 1 ? "Итоговые тесты" : "Итоговый тест";

                    Debug.Log($"{group.Key}. {testWord} {tests}");

                    needTests += $"{group.Key}. {testWord} {tests} \n";

                }
                TestsTextDisplay.text = needTests;
                //Debug.Log(string.Join("\n", ListNeedTests));
            }

            if (index >= 0 && index < multiLineText.hoverTexts.Length)
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
        TestsTextDisplay.text = string.Empty;
    }
}
