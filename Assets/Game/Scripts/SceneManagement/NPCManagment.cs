using System.Collections.Generic;
using Core.Quests;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class NPCManagment : MonoBehaviour
    {
        private Dictionary<int, GameObject> _npcTests;

        [FormerlySerializedAs("notComplite")] [SerializeField]
        private List<int> _notComplete;

        public List<int> NotComplete => _notComplete;

        public void Init() // TODO construct
        {
            SceneManager.sceneLoaded += SceneLoader_LevelChanged;
        }

        public bool checkAllTestsComplite()
        {
            _notComplete = new List<int>();

            FindAllNPCWithTests();

            foreach (int itemTestId in _npcTests.Keys)
            {
                bool complete = false;
                foreach (OneLeson lesson in IGame.Instance.dataPlayer.PlayerData.progress)
                {
                    foreach (OneTestQuestion test in lesson.tests)
                    {
                        if (_npcTests.ContainsKey(test.id))
                        {
                            if (test.id == itemTestId)
                            {
                                complete = test.completed;
                            }
                        }
                    }
                }

                if (complete == false)
                {
                    _notComplete.Add(itemTestId);
                }
            }

            if (_notComplete.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        private void SceneLoader_LevelChanged(Scene arg0, LoadSceneMode arg1)
        {
            UpdateAllNPCinScene();
        }

        private void FindAllNPCWithTests()
        {
            _npcTests = new Dictionary<int, GameObject>();

            NPC_for_testID[] conversationStarters = FindObjectsOfType<NPC_for_testID>();

            foreach (NPC_for_testID item in conversationStarters)
            {
                if (item.TestID > 0)
                {
                    _npcTests[item.TestID] = item.gameObject;
                }
            }
        }
        
        private void UpdateAllNPCinScene()
        {
            FindAllNPCWithTests();

            foreach (OneLeson lesson in IGame.Instance.dataPlayer.PlayerData.progress)
            {
                foreach (OneTestQuestion test in lesson.tests)
                {
                    if (_npcTests.ContainsKey(test.id))
                    {
                        _npcTests[test.id].transform.parent.gameObject.SetActive(!test.completed);
                    }
                }
            }
        }
    }
}