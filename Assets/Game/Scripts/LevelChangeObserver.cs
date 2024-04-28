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
        town2,
        holl,
        battleScene2

    }

    [Serializable]
    public class OneScene
    {
        public allScenes IdScene;
        public UnityEngine.Object fileScene;
    }

    [SerializeField] public List<OneScene> AllScenes=new List<OneScene>();
    public Dictionary<allScenes, UnityEngine.Object> DAllScenes;


    /*Vector3[] spawnPoints = new Vector3[]
    {new Vector3(0, 0, 0),
        new Vector3(-15.1000004f, 0f, -7.69999981f),   // Первая точка спауна
        new Vector3(-37.5600014f,-0.0500000007f,51.6399994f),   // Вторая точка спауна
        new Vector3(2.68000007f,-1f,23.5499992f),
        new Vector3(0.699999988f,-0.150000095f,1.39999998f),
        new Vector3(196.570007f,-23.9200001f,36.7700005f),
        new Vector3(210.96077f,-9.77070045f,148.226288f),
        new Vector3(29.2999992f,-7f,-109f)
    };*/
    Vector3[] spawnPointsSavePoint = new Vector3[]
    {
        new Vector3(196.570007f,-23.9200001f,36.7700005f),
        new Vector3(151.440002f,-18.0779991f,-20.8799992f),
        new Vector3(125.93f,-15.4200001f,-87.1699982f),
        new Vector3(79.8099976f,-16.1599998f,3.08999991f),
        new Vector3(27.9099998f,-24.5699997f,100.440002f)
    };

    [SerializeField] DataPlayer data;

    private void Start()
    {
        DAllScenes = new Dictionary<allScenes, UnityEngine.Object>();
        foreach (OneScene scene in AllScenes)
        {
            if (DAllScenes.ContainsKey(scene.IdScene))
                Debug.LogError("scene already exist in manager");
            DAllScenes[scene.IdScene] = scene.fileScene;
        }

        // Подписываемся на событие изменения уровня загрузки.
        RPG.Core.SceneLoader.AddEventListenerLevelChange(OnLevelChanged);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        allScenes newLevel = allScenes.emptyScene;
        foreach (var item in IGame.Instance.LevelChangeObserver.DAllScenes)
        {
            if (item.Value.name == arg0.name)
            {
                newLevel = item.Key;
            }
        }
        if (newLevel == allScenes.battle1)
        {
            UpdatePlayerLocation(spawnPointsSavePoint[IGame.Instance.dataPLayer.playerData.spawnPoint]);
            Debug.Log("Загружена 5 сцена сюда можно добавить условие");
        }
        else
        {
            //var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "StartPoint"); ;

            GameObject StartPos = GameObject.Find("StartPoint");

            if (StartPos != null)
            {
                Vector3 position = StartPos.transform.position;
                UpdatePlayerLocation(position);
            }

            //UpdatePlayerLocation(spawnPoints[newLevel]);
        }

        RPG.SceneManagement.SavePointsManager.UpdateStateSpawnPointsAfterLoad(IGame.Instance.dataPLayer,true);
        IGame.Instance.dataPLayer.playerData.chargeEnergy = 0;

        StartCoroutine(IGame.Instance.gameAPI.SaveGameData(IGame.Instance.dataPLayer.playerData));
    }

    // Метод, вызываемый при изменении уровня загрузки.
    private void OnLevelChanged(allScenes newLevel)
    {
        Debug.Log("Уровень загрузки изменен на " + newLevel);
        // Загружаем сцену с измененным номером.
        SavePointsManager.ResetDict();
        SceneManager.LoadScene(DAllScenes[newLevel].name);

    }

    private void OnDestroy()
    {
        // Не забудьте отписаться при уничтожении объекта.
        RPG.Core.SceneLoader.RemoveEventListenerLevelChange(OnLevelChanged);
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

}
