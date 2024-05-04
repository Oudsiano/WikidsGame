using RPG.Controller;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    public static IGame Instance;

    public DataPlayer dataPLayer;
    public GameAPI gameAPI;

    public PlayerController playerController;
    public LevelChangeObserver LevelChangeObserver;
    public SavePointsManager SavePointsManager;
    public ArrowForPlayerManager ArrowForPlayerManager;

    [SerializeField] public UIManager UIManager;
    [SerializeField] public CoinManager CoinManager;
    [SerializeField] public BottleManager BottleManager;
    [SerializeField] public WeaponManager WeaponManager;


    private void Awake()
    {
        if (Instance == null)

        {
            Instance = this;

            dataPLayer = FindObjectOfType<DataPlayer>();
            gameAPI = FindObjectOfType<GameAPI>();
            playerController = FindObjectOfType<PlayerController>();
            LevelChangeObserver = FindAnyObjectByType<LevelChangeObserver>();
            BottleManager = FindAnyObjectByType<BottleManager>();
            WeaponManager = FindAnyObjectByType<WeaponManager>();
            SavePointsManager = new SavePointsManager();
            ArrowForPlayerManager = new ArrowForPlayerManager();

            UIManager.Init();
            LevelChangeObserver.Init();
            playerController.Init();
            ArrowForPlayerManager.Init();
        }
    }
}
