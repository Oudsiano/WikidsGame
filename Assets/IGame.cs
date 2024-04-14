using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    public static IGame Instance;

    public LevelChangeObserver LChanger;

    [SerializeField] public UIManager UIManager;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        UIManager.Init();
    }
}
