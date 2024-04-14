using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    public static IGame Instance;

    public LevelChangeObserver LChanger;
    public DataPlayer dataPLayer;

    [SerializeField] public UIManager UIManager;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        dataPLayer = FindObjectOfType<DataPlayer>();

        UIManager.Init();
    }
}
