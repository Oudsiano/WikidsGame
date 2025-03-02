using System.Collections.Generic;
using Core.Player;
using Cysharp.Threading.Tasks;
using Data;
using Healths;
using Loading;
using Loading.LoadingOperations;
using Saving;
using SceneManagement.Enums;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class LevelChangeObserver : MonoBehaviour // TODO check
    {
        private int _indexSceneToLoad = 0;
        private SavePointsManager _savePointsManager;
        private DataPlayer _dataPlayer;
        private UIManager _uiManager;
        private MainPlayer _player;
        private GameAPI _gameAPI;
        private LoadingScreenProvider _loadingScreenProvider;

        public int IndexSceneToLoad => _indexSceneToLoad;

        public void Construct(SavePointsManager savePointsManager, DataPlayer dataPlayer, UIManager uiManager,
            MainPlayer player, GameAPI gameAPI, LoadingScreenProvider loadingScreenProvider)
        {
            _savePointsManager = savePointsManager;
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;
            _player = player;
            _gameAPI = gameAPI;
            _loadingScreenProvider = loadingScreenProvider;

            // Подписываемся на событие изменения уровня загрузки.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_dataPlayer.PlayerData.spawnPoint == 0)
            {
                GameObject startPos = GameObject.Find("StartPoint");

                if (startPos != null)
                {
                    UpdatePlayerLocation(startPos.transform.position, startPos.transform.rotation);
                    _uiManager.FollowCamera.ActivateCommonZoomUpdate();
                }
            }
            else if (SavePointsManager.AllSavePoints.Count > 0)
            {
                Vector3 pos = SavePointsManager.AllSavePoints[_dataPlayer.PlayerData.spawnPoint].transform.position;
                UpdatePlayerLocation(pos, Quaternion.identity);
                _uiManager.FollowCamera.ActivateCommonZoomUpdate();
            }

            _savePointsManager.UpdateStateSpawnPointsAfterLoad(true);
            _player.ResetCountEnergy();
            _gameAPI.SaveUpdater();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void TryChangeLevel(int indexScene, int newSpawnPoint)
        {
            _savePointsManager.ResetDict();
            _dataPlayer.PlayerData.spawnPoint = newSpawnPoint;
            LoadLevel(indexScene);
        }

        public void UpdateCurrentLevel()
        {
            LoadLevel(_dataPlayer.PlayerData.sceneToLoad);
        }

        public void OnFadeComplete()
        {
            SceneManager.LoadScene(_indexSceneToLoad);
        }

        private void LoadLevel(int newIndex)
        {
            _indexSceneToLoad = newIndex;
            Debug.Log("Уровень загрузки изменен на " + newIndex);
            
            _loadingScreenProvider.LoadAndDestroy(new NextSceneOperation(_indexSceneToLoad)).Forget();
        }

        // Метод для обновления местоположения игрока
        private void UpdatePlayerLocation(Vector3 spawnPoint, Quaternion rotation)
        {
            NavMeshAgent agent = _player.gameObject.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.enabled = false;

            _player.transform.position = spawnPoint;
            _player.transform.rotation = rotation;

            Animator animator = _player.gameObject.GetComponent<Animator>();

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }

            _player.gameObject.GetComponent<Health>()?.Restore();

            if (agent != null)
                agent.enabled = true;
        }
    }
}