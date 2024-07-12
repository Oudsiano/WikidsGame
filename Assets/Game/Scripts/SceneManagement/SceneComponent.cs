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

}
