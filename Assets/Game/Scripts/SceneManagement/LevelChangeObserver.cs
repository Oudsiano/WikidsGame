using System.Collections.Generic;
using Core.Health;
using Core.Player;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class LevelChangeObserver : MonoBehaviour // TODO check
    {
        [SerializeField] public List<OneScene> AllScenes = new();

        public Dictionary<allScenes, string> DAllScenes;

        private Dictionary<allScenes, bool>
            dictForInfected = new(); //типа зеленые/красные кнопки в сыборе локации сюда запишутся дополнительно

        [SerializeField] public DataPlayer data;

        public Dictionary<allScenes, bool> DictForInfected
        {
            get => dictForInfected;
            set => dictForInfected = value;
        }

        public void Init()
        {
            DAllScenes = new Dictionary<allScenes, string>();
            foreach (OneScene scene in AllScenes)
            {
                if (DAllScenes.ContainsKey(scene.IdScene))
                {
                    Debug.LogError("scene already exist in manager");
                }

                DAllScenes[scene.IdScene] = scene.fileScene;
            }

            // Подписываемся на событие изменения уровня загрузки.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public allScenes GetCurrentSceneId()
        {
            string currentSceneName = SceneManager.GetActiveScene().name; // TODO sceneLoader here

            // Ищем объект OneScene, соответствующий текущей сцене
            foreach (var sceneInfo in AllScenes)
            {
                if (sceneInfo.fileScene == currentSceneName)
                {
                    return sceneInfo.IdScene;
                }
            }

            // Если сцена не найдена в списке, можно вернуть null или выполнить другую логику
            if (currentSceneName != "OpenScene")
            {
                Debug.LogError("ErRrOrR Current scene info not found in AllScenes list: " + currentSceneName);
            }

            return allScenes.emptyScene;
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

            if (IGame.Instance.dataPlayer.PlayerData.spawnPoint == 0)
            {
                GameObject StartPos = GameObject.Find("StartPoint");

                if (StartPos != null)
                {
                    Vector3 position = StartPos.transform.position;
                    Quaternion rotation = StartPos.transform.rotation;
                    UpdatePlayerLocation(position, rotation);
                    IGame.Instance.UIManager.FollowCamera.ActivateCommonZoomUpdate();
                }
            }
            else if (SavePointsManager.AllSavePoints.Count > 0)
            {
                Vector3 posThere = SavePointsManager.AllSavePoints[IGame.Instance.dataPlayer.PlayerData.spawnPoint]
                    .transform.position;

                Quaternion rotation = new Quaternion();
                UpdatePlayerLocation(posThere, rotation);
                IGame.Instance.UIManager.FollowCamera.ActivateCommonZoomUpdate();
            }

            SavePointsManager.UpdateStateSpawnPointsAfterLoad(IGame.Instance.dataPlayer, true);
            MainPlayer.Instance.ResetCountEnergy();

            IGame.Instance.gameAPI.SaveUpdater();
        }

        // Метод для обновления местоположения игрока
        private void UpdatePlayerLocation(Vector3 spawnPoint, Quaternion rotation)
        {
            // Отключаем навигацию для игрока
            MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = false; // TODO can be cached

            // Устанавливаем позицию игрока в соответствии с порталом назначения
            MainPlayer.Instance.transform.position = spawnPoint;
            MainPlayer.Instance.transform.rotation = rotation;

            Animator animator =
                MainPlayer.Instance.gameObject.GetComponent<Animator>();
            animator.Rebind();
            animator.Update(0f);

            MainPlayer.Instance.gameObject.GetComponent<Health>().Restore();

            // Включаем навигацию для игрока
            MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}