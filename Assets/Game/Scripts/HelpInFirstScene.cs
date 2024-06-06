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
        Panel.SetActive(false);
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
        Panel.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 0) return;
        IGame.Instance.dataPLayer.playerData.helpIndex = 1;
        Study2();
    }


    public void Study2()
    {
        if (IGame.Instance.dataPLayer.playerData.sceneToLoad != (int)allScenes.battle1) return;
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 1) return;
        restTexts();
        text2.SetActive(true);
        Panel.SetActive(true);
    }
    private void EndStudy2()
    {
        Panel.SetActive(false);
        text2.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 1) return;
        IGame.Instance.dataPLayer.playerData.helpIndex = 2;
    }
    public void Study3()
    {
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 2) return;
        restTexts();
        text3.SetActive(true);
        Panel.SetActive(true);
    }

    public void EndStudy3()
    {
        Panel.SetActive(false);
        text3.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 2) return;
        IGame.Instance.dataPLayer.playerData.helpIndex = 3;
    }
    public void Study4()
    {
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 3) return;
        restTexts();
        text4.SetActive(true);
        Panel.SetActive(true);
    }
    public void EndStudy4()
    {
        Panel.SetActive(false);
        text4.SetActive(false);
        if (IGame.Instance.dataPLayer.playerData.helpIndex != 3) return;
        IGame.Instance.dataPLayer.playerData.helpIndex = 4;
    }
    public void Study5()
    {
        //��� ��� �������. ���� ����� �� ����� ����� � ������
        restTexts();
        text5.SetActive(true);
        Panel.SetActive(true);
        IGame.Instance.dataPLayer.playerData.helpIndex = 5;
    }
    public void EndStudy5()
    {
        Panel.SetActive(false);
        text5.SetActive(false);
    }


}
