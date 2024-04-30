using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelChangeObserver;

public class HelpInFirstScene : MonoBehaviour
{
    


    [SerializeField] GameObject Panel;
    [SerializeField] GameObject text1;
    [SerializeField] GameObject text2;
    [SerializeField] GameObject text3;
    [SerializeField] GameObject text4;
    [SerializeField] GameObject text5;


    private void Awake()
    {
        RPG.Core.SceneLoader.LevelChanged += SceneLoader_LevelChanged;

        RPG.Core.FollowCamera.OnCameraRotation += FollowCamera_OnCameraRotation;

        RPG.Core.FollowCamera.OnCameraScale += FollowCamera_OnCameraScale;
        restTexts();
    }

    private void SceneLoader_LevelChanged(allScenes s)
    {
        Study1Show(s);
    }

    private void FollowCamera_OnCameraRotation() => EndStudy1();
    private void FollowCamera_OnCameraScale() => EndStudy2();

    private void OnDestroy()
    {
        RPG.Core.FollowCamera.OnCameraRotation -= FollowCamera_OnCameraRotation;
        RPG.Core.FollowCamera.OnCameraScale -= FollowCamera_OnCameraScale;
    }

    private void restTexts()
    {
        Panel.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5.SetActive(false);
    }

    public void Study1Show(allScenes s)
    {
        if (s != allScenes.battle1) return;
        if (IGame.Instance.dataPLayer.playerData.helpIndex !=0) return;

        if (Panel == null)
        {
            //Debug.LogError("panel null");
            return;
        }

        restTexts();
        Panel.SetActive(true);
        text1.SetActive(true);

    }

    private void EndStudy1()
    {
        text1.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 0) return;
        Panel.SetActive(false);
        IGame.Instance.dataPLayer.playerData.helpIndex = 1;
        Study2();
    }


    public void Study2()
    {
        if (IGame.Instance.dataPLayer.playerData.sceneToLoad != (int)allScenes.battle1) return;
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 1) return;
        restTexts();
        Panel.SetActive(true);
        text2.SetActive(true);
    }
    private void EndStudy2()
    {
        text2.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 1) return;
        Panel.SetActive(false);
        IGame.Instance.dataPLayer.playerData.helpIndex = 2;
    }
    public void Study3()
    {
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 2) return;
        restTexts();
        Panel.SetActive(true);
        text3.SetActive(true);
    }

    public void EndStudy3()
    {
        text3.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 2) return;
        Panel.SetActive(false);
        IGame.Instance.dataPLayer.playerData.helpIndex = 3;
    }
    public void Study4()
    {
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 3) return;
        restTexts();
        Panel.SetActive(true);
        text4.SetActive(true);
    }
    public void EndStudy4()
    {
        text4.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 3) return;
        Panel.SetActive(false);
        IGame.Instance.dataPLayer.playerData.helpIndex = 4;
    }
    public void Study5()
    {
        //тут без условия. Типа дошли до конца карты и хватит
        restTexts();
        Panel.SetActive(true);
        text5.SetActive(true);
            IGame.Instance.dataPLayer.playerData.helpIndex = 5;
    }
    public void EndStudy5()
    {
        text5.SetActive(false);
        Panel.SetActive(false);
    }


}
