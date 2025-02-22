using System;
using Saving;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneManagement.LevelChangeObserver;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour // TODO in SCENELOADER SERVICE
    {
        // Определите делегат для события изменения уровня загрузки.
        public delegate void LevelChangedEventHandler(allScenes IdNewLevel); 
        // Событие, возникающее при изменении уровня загрузки.
        public static event Action<allScenes> LevelChanged;
        
        [SerializeField] private allScenes levelToLoad = 0; // TODO rename
        private static SceneLoader _instance; // TODO rename

        public static SceneLoader Instance // TODO rename 
        {
            get { return _instance; }
        }

        public void Construct()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public void UpdateCurrentLevel()
        {
            LoadLevel((allScenes)IGame.Instance.dataPlayer.PlayerData.sceneToLoad);
        }

        public void TryChangeLevel(allScenes IdNewLevel,int newSpawnPoint)
        {
            SavePointsManager.ResetDict();
            IGame.Instance.dataPlayer.PlayerData.spawnPoint = newSpawnPoint;
            LoadLevel(IdNewLevel);
        }
        
        public void OnFadeComplete()
        {
            SceneManager.LoadScene(IGame.Instance.LevelChangeObserver.DAllScenes[levelToLoad]);
        }
        
        private void LoadLevel(allScenes IdNewLevel)
        {
            if (IdNewLevel == allScenes.emptyScene)
            {
                Debug.LogWarning("Forgotten add scene somewhere");
            }

            levelToLoad = IdNewLevel;
            // При изменении уровня загрузки вызываем событие.

            Debug.Log("Уровень загрузки изменен на " + IdNewLevel);
            // Загружаем сцену с измененным номером.
            SceneManager.LoadScene(IGame.Instance.LevelChangeObserver.DAllScenes[IdNewLevel]);

            OnLevelChanged(IdNewLevel);
        }
        
        // Вызываем событие при изменении уровня загрузки.
        private void OnLevelChanged(allScenes IdNewLevel)
        {
            LevelChanged?.Invoke(IdNewLevel);
        }
    }
}
