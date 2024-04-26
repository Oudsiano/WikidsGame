using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardsEvents : MonoBehaviour
{
    public enum EscState
    {
        none,
        againScr
    }

    public enum SceneState
    {
        meny,
        battle
    }

    public static EscState escState;
    public static SceneState sceneState;

    private void Awake()
    {
        RPG.Core.SceneLoader.AddEventListenerLevelChange((e) =>
        {
            if (e == 0)
                KeyBoardsEvents.sceneState = SceneState.meny;
            else KeyBoardsEvents.sceneState = SceneState.battle;
        });
    }



    private void changeEscState()
    {
        if (sceneState == SceneState.battle)
        {
            if (IGame.Instance.playerController.PlayerUIManager.MapCanvas.gameObject.activeSelf)
            {
                IGame.Instance.playerController.PlayerUIManager.MapCanvas.gameObject.SetActive(false);
            }
            else
            switch (escState)
            {
                case EscState.none:
                    IGame.Instance.UIManager.ShowAgainUi();
                    break;
                case EscState.againScr:
                    IGame.Instance.UIManager.OnCLickCancelAgain();
                    break;
                default:
                    break;
            }
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            changeEscState();
        }
    }
}
