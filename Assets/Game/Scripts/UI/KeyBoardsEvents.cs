using SceneManagement;
using SceneManagement.Enums;
using UI.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class KeyBoardsEvents : MonoBehaviour
    {
        public static EscState escState; // TODO static
        public static SceneState sceneState; // TODO static

        private SceneLoaderService _sceneLoader;
        private UIManager _uiManager;

        public void Construct(SceneLoaderService sceneLoader, UIManager uiManager)
        {
            _sceneLoader = sceneLoader;
            _uiManager = uiManager;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeEscState();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            sceneState = scene.buildIndex == _sceneLoader.OpenScene ? SceneState.Menu : SceneState.Battle;
        }

        private void ChangeEscState()
        {
            if (sceneState == SceneState.Battle)
            {
                if (_uiManager.MapCanvas.gameObject.activeSelf)
                {
                    _uiManager.OnClickBtnCloseMap();
                }
                else
                    switch (escState)
                    {
                        case EscState.none:
                            _uiManager.ShowAgainUi();
                            break;

                        case EscState.againScr:
                            _uiManager.OnCLickCancelAgain();
                            break;
                    }
            }
        }
    }
}