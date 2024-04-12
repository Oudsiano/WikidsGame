using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Core;
using UnityEngine.AI;

public class LevelChangeObserver : MonoBehaviour
{
    Vector3[] spawnPoints = new Vector3[]
    {
        new Vector3(-15.1000004f, 0f, -7.69999981f),   // Первая точка спауна
        new Vector3(-37.5600014f,-0.0500000007f,51.6399994f),   // Вторая точка спауна
        new Vector3(2.68000007f,-1f,23.5499992f),
        new Vector3(0.699999988f,-0.150000095f,1.39999998f),
        new Vector3(196.570007f,-23.9200001f,36.7700005f),
        new Vector3(210.96077f,-9.77070045f,148.226288f)
    };
    Vector3[] spawnPointsSavePoint = new Vector3[]
    {
        new Vector3(196.570007f,-23.9200001f,36.7700005f),
        new Vector3(151.440002f,-18.0779991f,-20.8799992f),
        new Vector3(125.93f,-15.4200001f,-87.1699982f),
        new Vector3(79.8099976f,-16.1599998f,3.08999991f),
        new Vector3(27.9099998f,-24.5699997f,100.440002f)
    };

    [SerializeField]DataPlayer data; 

    private void Start()
    {
        // Подписываемся на событие изменения уровня загрузки.
        RPG.Core.SceneLoader.AddEventListenerLevelChange ( OnLevelChanged);
    }

    // Метод, вызываемый при изменении уровня загрузки.
    private void OnLevelChanged(int newLevel)
    {
        Debug.Log("Уровень загрузки изменен на " + newLevel);
        // Загружаем сцену с измененным номером.
        SceneManager.LoadScene(newLevel);
        data = FindObjectOfType<DataPlayer>();
        if(newLevel == 4 )
        {
            UpdatePlayerLocation(spawnPointsSavePoint[data.playerData.spawnPoint]);
            Debug.Log("Загружена 4 сцена сюда можно добавить условие");
        } else
        {
            UpdatePlayerLocation(spawnPoints[newLevel]);
        }

        RPG.SceneManagement.SavePointsManager.UpdateStateSpawnPointsAfterLoad(data);
    }

    private void OnDestroy()
    {
        // Не забудьте отписаться при уничтожении объекта.
        RPG.Core.SceneLoader.RemoveEventListenerLevelChange ( OnLevelChanged);
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
