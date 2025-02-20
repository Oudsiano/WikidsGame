using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelChangeObserver;

public class SceneComponent : MonoBehaviour
{
    [SerializeField] public allScenes IdScene;

    //[SerializeField] public GameObject TheCore;

    [Header("Camera New Params")]
    [SerializeField] public int newMinZoomCamera;
    [SerializeField] public int newMaxZoomCamera;

    [Header("Infection Group")]
    [SerializeField] public GameObject InfectGroup;

    private void Awake()
    {
        if (InfectGroup == null)
        {
            InfectGroup = GameObject.Find("infection");
            if (InfectGroup == null)
                InfectGroup = GameObject.Find("infested");
        }
        if (IGame.Instance.LevelChangeObserver.DictForInfected.ContainsKey(IdScene))
            SetInfectGroupActive(IGame.Instance.LevelChangeObserver.DictForInfected[IdScene]);
    }

    public void SetInfectGroupActive(bool isActive)
    {
        if (InfectGroup != null)
            InfectGroup.SetActive(isActive);
    }
}
