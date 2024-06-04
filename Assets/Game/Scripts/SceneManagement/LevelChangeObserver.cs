using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Core;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Linq;
using RPG.SceneManagement;

public class LevelChangeObserver : MonoBehaviour
{
    public enum allScenes
    {
        emptyScene,
        regionSCene,
        battle1,
        town1,
        library,
        holl,
        battleScene1,
        BS3,
        VikingScene,

    }

    [Serializable]
    public class OneScene
    {
        [SerializeField]
        public allScenes IdScene;
        [SerializeField]
        public string fileScene;
    }

    [SerializeField] 
    public List<OneScene> AllScenes=new List<OneScene>();

    public Dictionary<allScenes, string> DAllScenes;

    /*Vector3[] spawnPointsSavePoint = new Vector3[]
    {
        new Vector3(196.570007f,-23.9200001f,36.7700005f),
        new Vector3(151.440002f,-18.0779991f,-20.8799992f),
        new Vector3(125.93f,-15.4200001f,-87.1699982f),
        new Vector3(79.8099976f,-16.1599998f,3.08999991f),
        new Vector3(27.9099998f,-24.5699997f,100.440002f)
    };*/

    [SerializeField] DataPlayer data;

    public void Init()
    {
        DAllScenes = new Dictionary<allScenes, string>();
        foreach (OneScene scene in AllScenes)
        {
            if (DAllScenes.ContainsKey(scene.IdScene))
                Debug.LogError("scene already exist in manager");
            DAllScenes[scene.IdScene] = scene.fileScene;
        }

        // Подписываемся на событие изменения уровня загрузки.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        allScenes newLevel = allScenes.emptyScene;
        foreach (var item in IGame.Instance.LevelChangeObserver.DAllScenes)
        {
            if (item.Value == arg0.name)
            {
                newLevel = item.Key;
            }
        }
        if (SavePointsManager.AllSavePoints.Count > 0)
        {
            Vector3 posThere = SavePointsManager.AllSavePoints[IGame.Instance.dataPLayer.playerData.spawnPoint].transform.position;
            UpdatePlayerLocation(posThere);
        }
        else
        {
            GameObject StartPos = GameObject.Find("StartPoint");

            if (StartPos != null)
            {
                Vector3 position = StartPos.transform.position;
                UpdatePlayerLocation(position);
            }

            //UpdatePlayerLocation(spawnPoints[newLevel]);
        }

        SavePointsManager.UpdateStateSpawnPointsAfterLoad(IGame.Instance.dataPLayer,true);
        MainPlayer.Instance.ResetCountEergy();

        IGame.Instance.gameAPI.SaveUpdater();
    }

    // Метод для обновления местоположения игрока
    private void UpdatePlayerLocation(Vector3 spawnPoint)
    {
        // Отключаем навигацию для игрока
        MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = false;

        // Устанавливаем позицию игрока в соответствии с порталом назначения
        MainPlayer.Instance.transform.position = spawnPoint;

        Animator anim =
            MainPlayer.Instance.gameObject.GetComponent<Animator>();
        anim.Rebind();
        anim.Update(0f);

        MainPlayer.Instance.gameObject.GetComponent<Health>().Restore();

        // Включаем навигацию для игрока
        MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }

    private void OnDestroy()
    {
        Debug.Log("destr");
    }
}
