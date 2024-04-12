using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core
{
    public class SceneLoader : MonoBehaviour
    {
        // Определите делегат для события изменения уровня загрузки.
        public delegate void LevelChangedEventHandler(int newLevel);
        // Событие, возникающее при изменении уровня загрузки.
        private static event LevelChangedEventHandler LevelChanged;

        [SerializeField] private int levelToLoad = -1;
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

        public void LoadScene(int levelToLoad, float timeToWait = 2f)
        {
            this.levelToLoad = levelToLoad;
            // При изменении уровня загрузки вызываем событие.
            OnLevelChanged(levelToLoad);

        }

        // Вызываем событие при изменении уровня загрузки.
        private void OnLevelChanged(int newLevel)
        {
            LevelChanged?.Invoke(newLevel);
        }

        public void OnFadeComplete()
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
