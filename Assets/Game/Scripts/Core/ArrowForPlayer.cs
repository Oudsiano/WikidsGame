using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowForPlayerManager
{
    

    private SortedDictionary<int, ArrowForPlayer> allArrowForPlayers;

    public SortedDictionary<int, ArrowForPlayer> AllArrowForPlayers { get => allArrowForPlayers; set => allArrowForPlayers = value; }

    public void Init()
    {
        allArrowForPlayers = new SortedDictionary<int, ArrowForPlayer>();
        SceneManager.sceneLoaded += SceneLoader_LevelChanged;
    }

    private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
    {
        allArrowForPlayers = new SortedDictionary<int, ArrowForPlayer>();
    }

    public void StartArrow()
    {
        List<ArrowForPlayer> sorted = AllArrowForPlayers.Values.ToList();
        if (sorted.Count > 0)
        {
            sorted[0].ArrowSprite.SetActive(true);
        }
    }

}


public class ArrowForPlayer : MonoBehaviour
{
    [SerializeField]
    public GameObject ArrowSprite;
    public GameObject ArrowImage;

    public int Index;

    private bool trigered = false;

    public void Init()
    {

    }

    private void Awake()
    {
        if (IGame.Instance!=null)
        IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[Index] = this;
        if (Index != 0)
        {
            ArrowSprite.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        trigered = true;

        for (int i = Index; i >= 0; i--)
        {
            if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.ContainsKey(i))
            if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].Index <= Index)
            {
                //if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.ContainsKey(IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].Index))
                {
                    IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].gameObject.SetActive(false);
                    IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.Remove(i);
                }
            }
        }

        IGame.Instance.ArrowForPlayerManager.StartArrow();
    }

    private void Update()
    {
        if (!trigered)
        {
            Vector3 rotate = transform.eulerAngles;
            Vector3 normPos = (transform.position - IGame.Instance.playerController.transform.position).normalized;
            float yAngle = Mathf.Acos(normPos.z) * Mathf.Rad2Deg;
            if (normPos.x < 0)
            {
                yAngle = -yAngle;
            }
            rotate.x = 90;
            rotate.y = yAngle;
            ArrowSprite.transform.rotation = Quaternion.Euler(rotate);

            ArrowSprite.transform.position = IGame.Instance.playerController.transform.position + new Vector3(0, 1, 0) + normPos * 3;
        }

    }

    private void OnDestroy()
    {
        Destroy(ArrowSprite);
        Destroy(ArrowImage);
    }
}
