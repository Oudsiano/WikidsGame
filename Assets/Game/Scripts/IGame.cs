using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    public static IGame Instance;

    public LevelChangeObserver LChanger;
    public DataPlayer dataPLayer;
    public GameAPI gameAPI;

    [SerializeField] public UIManager UIManager;
    [SerializeField] public CoinManager CoinManager;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        dataPLayer = FindObjectOfType<DataPlayer>();
        gameAPI = FindObjectOfType<GameAPI>();

        UIManager.Init();
    }
}
