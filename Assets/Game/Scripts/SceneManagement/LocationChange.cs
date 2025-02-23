using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Quests;
using Core.Quests.Data;
using Data;
using Saving;
using SceneManagement.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneManagement
{
    public class LocationChange : MonoBehaviour
    {
        [FormerlySerializedAs("regions")] [SerializeField]
        private List<OneBtnChangeRegion> _regions = new();

        [FormerlySerializedAs("Loading")] [SerializeField]
        private GameObject _loading; // TODO GO

        [FormerlySerializedAs("hoverTextDisplay")] [SerializeField]
        private TMP_Text _hoverTextDisplay; // Поле для отображения текста при наведении

        [FormerlySerializedAs("TestsTextDisplay")] [SerializeField]
        private TMP_Text _testsTextDisplay;

        [FormerlySerializedAs("multiLineText")] [SerializeField]
        private MultiLineText _multiLineText; // Ссылка на компонент с многострочным текстом

        [FormerlySerializedAs("textID")] [SerializeField]
        private TMP_Text _textID;

        [FormerlySerializedAs("continueButton")] [SerializeField]
        private Button _continueButton;

        private SceneWithTestsID _sceneWithTestsID;
        private List<string> _listNeedTests;

        private DataPlayer _dataPlayer;
        private LevelChangeObserver _levelChangeObserver;
        private SceneLoader _sceneLoader;
        private GameAPI _gameAPI;
        
        public void Construct(DataPlayer dataPlayer, LevelChangeObserver levelChangeObserver, SceneLoader sceneLoader,
            GameAPI gameAPI)
        {
            _dataPlayer = dataPlayer;
            _levelChangeObserver = levelChangeObserver;
            _sceneLoader = sceneLoader;
            _gameAPI = gameAPI;
            
            if (_multiLineText.hoverTexts.Length != _regions.Count)
            {
                Debug.LogError("The number of hover texts does not match the number of regions.");
                return;
            }

            for (int i = 0; i < _regions.Count; i++)
            {
                int index = i; // Локальная копия переменной, чтобы избежать проблем с замыканием
                _regions[i].Button.onClick.AddListener(() => OnClick(_regions[index].loadedScene));

                EventTrigger trigger = _regions[i].Button.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entryEnter = new EventTrigger.Entry();
                entryEnter.eventID = EventTriggerType.PointerEnter;
                entryEnter.callback.AddListener((eventData) => { OnPointerEnter(index); });
                trigger.triggers.Add(entryEnter);

                EventTrigger.Entry entryExit = new EventTrigger.Entry();
                entryExit.eventID = EventTriggerType.PointerExit;
                entryExit.callback.AddListener((eventData) => { OnPointerExit(); });
                trigger.triggers.Add(entryExit);
            }


            _sceneWithTestsID = FindObjectOfType<SceneWithTestsID>(); // TODO find change
            UpdateColors();
            SetupMaxRegion(_dataPlayer.PlayerData.FinishedRegionsIDs);

            if (_textID != null)
            {
                _textID.text = _dataPlayer.PlayerData.id.ToString();
            }

            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnClickLoadFromSaveState);
            }

            if (_dataPlayer.PlayerData.sceneToLoad > 0)
            {
                _continueButton.gameObject.SetActive(true);
            }
            else
            {
                _continueButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            _loading.gameObject.SetActive(false);
        }

        private void SetupMaxRegion(List<int> n)
        {
            int findedIndex = -1;

            for (int i = _regions.Count - 1; i >= 0; i--)
            {
                if (n.Contains((int)_regions[i].loadedScene))
                {
                    findedIndex = i;

                    break;
                }
            }

            if (findedIndex + 1 < _regions.Count) // TODO magic numbers
            {
                findedIndex++;
            }

            for (int i = 0; i < _regions.Count; i++)
            {
                if (i <= findedIndex)
                {
                    _regions[i].Button.interactable = true;
                }
                else
                {
                    _regions[i].Button.interactable = false;
                    _regions[i].SetNormal();
                }

                if (i == findedIndex)
                {
                    _regions[i].SetNormal();
                }
            }
        }

        private void UpdateColors()
        {
            var playerData = _dataPlayer.PlayerData.progress;

            foreach (OneBtnChangeRegion region in _regions)
            {
                region.SetGreen();
                _levelChangeObserver.DictForInfected[region.loadedScene] = false;

                // Находим соответствующую сцену для текущего региона.
                var sceneData = _sceneWithTestsID.SceneDataList
                    .FirstOrDefault(scene => scene.scene == region.loadedScene);

                if (sceneData != null && playerData != null)
                {
                    // Проверяем тесты текущей сцены.
                    var incompleteTestFound = sceneData.numbers // TODO can be allocated call memory
                        .Any(testScene => playerData
                            .Any(lesson => lesson.tests
                                .Any(test => test.id == testScene && !test.completed)));

                    // Если найден незавершенный тест, устанавливаем цвет в красный.
                    if (incompleteTestFound)
                    {
                        region.SetRed();
                        _levelChangeObserver.DictForInfected[region.loadedScene] = true;
                        continue;
                    }
                }
            }
        }

        private void OnClickLoadFromSaveState() // TODO rename
        {
            _loading.gameObject.SetActive(true);
            //_sceneLoader.TryChangeLevel((allScenes)2, 4);
            _sceneLoader.TryChangeLevel((allScenes)_dataPlayer.PlayerData.sceneToLoad,
                _dataPlayer.PlayerData.spawnPoint);
            AudioManager.instance.PlaySound("ClickButton");
        }

        private void OnClick(allScenes sceneId)
        {
            _dataPlayer.SetSceneToLoad(sceneId);
            _loading.gameObject.SetActive(true);
            _gameAPI.SaveUpdater();
            //Invoke("LoadSceneAfterDelay", 2f); 
            _sceneLoader.TryChangeLevel(sceneId, 0);
            AudioManager.instance.PlaySound("ClickButton"); // TODO can be cached
        }

        private void OnPointerEnter(int index) // TODO SUPPPPPPER OVERLOAD METHOD
        {
            String needTests = "";
            _listNeedTests = new List<string>();

            if (index >= 0 && index < _regions.Count)
            {
                if (_regions[index].Button.interactable == true)
                {
                    foreach (SceneData scene in _sceneWithTestsID.SceneDataList)
                    {
                        if (scene.scene == _regions[index].loadedScene)
                        {
                            foreach (int testScene in scene.numbers)
                            {
                                if (_dataPlayer.PlayerData.progress != null)
                                {
                                    foreach (OneLeson item in _dataPlayer.PlayerData.progress)
                                    {
                                        foreach (OneTestQuestion item2 in item.tests)
                                        {
                                            if (testScene == item2.id)
                                            {
                                                if (item2.completed == false)
                                                {
                                                    _listNeedTests.Add(item2.title);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var grouped = _listNeedTests
                        .Select(s => new
                        {
                            Original = s,
                            Key = Regex.Replace(s, @" Тест \d+| Итоговый тест \d+",
                                ""), // TODO can be cached and optimized grouped
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
                        string testWord = group.TestNumbers.Count > 1 ? "Тесты" : "Тест"; // TODO can be cached 
                        if (group.TestType == "Итоговый тест") // TODO can be cached 
                            testWord = group.TestNumbers.Count > 1
                                ? "Итоговые тесты"
                                : "Итоговый тест"; // TODO can be cached 

                        Debug.Log($"{group.Key}. {testWord} {tests}");

                        needTests += $"{group.Key}. {testWord} {tests} \n"; // TODO can be cached 
                    }

                    _testsTextDisplay.text = needTests;
                    //Debug.Log(string.Join("\n", ListNeedTests));
                }

                if (index >= 0 && index < _multiLineText.hoverTexts.Length)
                {
                    _hoverTextDisplay.text = _multiLineText.hoverTexts[index];
                }
            }
            else
            {
                Debug.LogWarning($"Index {index} is out of range for hover texts.");
            }
        }

        private void OnPointerExit()
        {
            _hoverTextDisplay.text = string.Empty;
            _testsTextDisplay.text = string.Empty;
        }
    }
}