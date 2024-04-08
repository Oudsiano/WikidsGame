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
    [SerializeField] GameAPI gameApi;

    private void Start()
    {
        // Подписываемся на событие изменения уровня загрузки.
        RPG.Core.SceneLoader.LevelChanged += OnLevelChanged;
    }

    // Метод, вызываемый при изменении уровня загрузки.
    private void OnLevelChanged(int newLevel)
    {
        Debug.Log("Уровень загрузки изменен на " + newLevel);
        // Загружаем сцену с измененным номером.
        SceneManager.LoadScene(newLevel);
        UpdatePlayerLocation(spawnPoints[newLevel]);
        gameApi.UpdateData();
    }

    private void OnDestroy()
    {
        // Не забудьте отписаться при уничтожении объекта.
        RPG.Core.SceneLoader.LevelChanged -= OnLevelChanged;
    }

    // Метод для обновления местоположения игрока
    private void UpdatePlayerLocation(Vector3 spawnPoint)
    {
        // Отключаем навигацию для игрока
        MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = false;

        // Устанавливаем позицию игрока в соответствии с порталом назначения
        MainPlayer.Instance.transform.position = spawnPoint;

        // Включаем навигацию для игрока
        MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }
}
