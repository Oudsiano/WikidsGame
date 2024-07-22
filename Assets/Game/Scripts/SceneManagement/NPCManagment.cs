using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManagment : MonoBehaviour
{
    private Dictionary<int, GameObject> _dicNPCTests;
    public List<int> notComplite;

    // Start is called before the first frame update

    public void Init()
    {
        SceneManager.sceneLoaded += SceneLoader_LevelChanged;
    }

    private void SceneLoader_LevelChanged(Scene arg0, LoadSceneMode arg1)
    {
        updateAllNPCinScene();
    }

    private void FindAllNPCWithTests()
    {
        //Need find all NPC
        _dicNPCTests = new Dictionary<int, GameObject>();

        NPC_for_testID[] conversationStarters = FindObjectsOfType<NPC_for_testID>();

        foreach (NPC_for_testID item in conversationStarters)
        {
            if (item.TestID > 0)
            {
                _dicNPCTests[item.TestID] = item.gameObject;
            }
        }
    }

    public bool checkAllTestsComplite()
    {
        notComplite = new List<int>();

        FindAllNPCWithTests();

        foreach (int itemTestId in _dicNPCTests.Keys)
        {
            bool complete = false;
            foreach (OneLeson lesson in IGame.Instance.dataPlayer.playerData.progress)
            {
                foreach (OneTestQuestion test in lesson.tests)
                {
                    if (_dicNPCTests.ContainsKey(test.id))
                    {
                        if (test.id == itemTestId)
                            complete = test.completed;
                    }
                }
            }

            if (!complete)
            {
                notComplite.Add(itemTestId);

            }

        }

        if (notComplite.Count > 0)
            return false;
        else
            return true;
    }

    public void updateAllNPCinScene()
    {
        FindAllNPCWithTests();

        foreach (OneLeson lesson in IGame.Instance.dataPlayer.playerData.progress)
        {
            foreach (OneTestQuestion test in lesson.tests)
            {
                if (_dicNPCTests.ContainsKey(test.id))
                {
                    _dicNPCTests[test.id].transform.parent.gameObject.SetActive(!test.completed);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
