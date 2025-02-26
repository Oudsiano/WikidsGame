using SceneManagement;
using SceneManagement.Enums;
using UI.Enums;
using UnityEngine;

namespace UI
{
    public class KeyBoardsEvents : MonoBehaviour
    {
        public static EscState escState; // TODO static
        public static SceneState sceneState; // TODO static

        private SceneLoader _sceneLoader;
        private UIManager _uiManager;

        public void Construct(SceneLoader sceneLoader, UIManager uiManager)
        {
            _sceneLoader = sceneLoader;
            _uiManager = uiManager;
            
            SceneLoader.LevelChanged += SceneLoader_LevelChanged; // Change static
        }
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeEscState();
            }
        }
    
        private void SceneLoader_LevelChanged(allScenes obj)
        {
            if (obj == 0)
            {
                sceneState = SceneState.meny;
            }
            else
            {
                sceneState = SceneState.battle;
            }
        }

        private void ChangeEscState()
        {
            if (sceneState == SceneState.battle)
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