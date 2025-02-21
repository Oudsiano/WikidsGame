using RPG.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneManagement.LevelChangeObserver;

namespace RPG.Core
{
    public class SceneLoader : MonoBehaviour
    {
        // Определите делегат для события изменения уровня загрузки.
        public delegate void LevelChangedEventHandler(allScenes IdNewLevel);
        // Событие, возникающее при изменении уровня загрузки.
        public static event Action<allScenes> LevelChanged;
        
        [SerializeField] private allScenes levelToLoad = 0;
        private static SceneLoader _instance;

        public static SceneLoader Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public void UpdateCurrentLevel()
        {
            LoadLevel((allScenes)IGame.Instance.dataPlayer.playerData.sceneToLoad);
        }

        public void TryChangeLevel(allScenes IdNewLevel,int newSpawnPoint)
        {
            SavePointsManager.ResetDict();
            IGame.Instance.dataPlayer.playerData.spawnPoint = newSpawnPoint;
            LoadLevel(IdNewLevel);
        }

        public void LoadLevel(allScenes IdNewLevel)
        {
            if (IdNewLevel == allScenes.emptyScene)
            {
                Debug.LogWarning("Forgotten add scene somewhere");
            }

            this.levelToLoad = IdNewLevel;
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


        public void OnFadeComplete()
        {
            SceneManager.LoadScene(IGame.Instance.LevelChangeObserver.DAllScenes[levelToLoad]);
        }

    }
}
