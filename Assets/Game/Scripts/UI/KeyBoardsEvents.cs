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

        private void Awake() // TODO construct
        {
            SceneLoader.LevelChanged += SceneLoader_LevelChanged;
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
                if (IGame.Instance._uiManager.MapCanvas.gameObject.activeSelf)
                {
                    IGame.Instance._uiManager.OnClickBtnCloseMap();
                }
                else
                    switch (escState)
                    {
                        case EscState.none:
                            IGame.Instance._uiManager.ShowAgainUi();
                            break;
                    
                        case EscState.againScr:
                            IGame.Instance._uiManager.OnCLickCancelAgain();
                            break;
                    }
            }
        }
    }
}