using RPG.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelChangeObserver;

namespace RPG.Core
{
    public class SceneLoader : MonoBehaviour
    {
        // Определите делегат для события изменения уровня загрузки.
        public delegate void LevelChangedEventHandler(allScenes IdNewLevel);
        // Событие, возникающее при изменении уровня загрузки.
        private static event LevelChangedEventHandler LevelChanged;

        [SerializeField] private allScenes levelToLoad = 0;
        private static SceneLoader _instance;

        public static SceneLoader Instance
        {
            get { return _instance; }
        }

        public static void AddEventListenerLevelChange(LevelChangedEventHandler e)
        {
            LevelChanged += e;
        }
        public static void RemoveEventListenerLevelChange(LevelChangedEventHandler e)
        {
            LevelChanged -= e;
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
            TryChangeLevel((allScenes)IGame.Instance.dataPLayer.playerData.sceneToLoad);
        }

        public void TryChangeLevel(allScenes IdNewLevel)
        {
            if (IdNewLevel == allScenes.emptyScene)
            {
                Debug.LogWarning("Forgotten add scene somewhere");
            }

            this.levelToLoad = IdNewLevel;
            // При изменении уровня загрузки вызываем событие.

            Debug.Log("Уровень загрузки изменен на " + IdNewLevel);
            // Загружаем сцену с измененным номером.
            SavePointsManager.ResetDict();
            SceneManager.LoadScene(IGame.Instance.LevelChangeObserver.DAllScenes[IdNewLevel].name);

            OnLevelChanged(IdNewLevel);
        }


        // Вызываем событие при изменении уровня загрузки.
        private void OnLevelChanged(allScenes IdNewLevel)
        {
            LevelChanged?.Invoke(IdNewLevel);
        }


        public void OnFadeComplete()
        {
            SceneManager.LoadScene(IGame.Instance.LevelChangeObserver.DAllScenes[levelToLoad].name);
        }

    }
}
