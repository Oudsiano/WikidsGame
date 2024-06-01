using RPG.Controller;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    private bool isPause = false;

    private static IGame instance;

    public DataPlayer dataPLayer;
    public GameAPI gameAPI;
    public SaveGame saveGame;

    public PlayerController playerController;
    public LevelChangeObserver LevelChangeObserver;
    public SavePointsManager SavePointsManager;
    public ArrowForPlayerManager ArrowForPlayerManager;
    public QuestManager QuestManager;

    [SerializeField] public UIManager UIManager;
    [SerializeField] public CoinManager CoinManager;
    [SerializeField] public BottleManager BottleManager;
    [SerializeField] public WeaponArmorManager WeaponArmorManager;

    public static IGame Instance { get {

            if (instance == null)
            {
                instance = FindObjectOfType<IGame>();
                instance.Init();
            }

                return instance;
                } 
        set => instance = value; }

    public bool IsPause { get => isPause; set => isPause = value; }

    /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private void Awake()
    {
        Debug.Log("inut Inst");
        if (Instance == null)

        {
            Instance = this;

            Init();
        }
    }*/

    private void Init()
    {
        dataPLayer = FindObjectOfType<DataPlayer>();
        gameAPI = FindObjectOfType<GameAPI>();
        playerController = FindObjectOfType<PlayerController>();
        LevelChangeObserver = FindAnyObjectByType<LevelChangeObserver>();
        BottleManager = FindAnyObjectByType<BottleManager>();
        WeaponArmorManager = FindAnyObjectByType<WeaponArmorManager>();
        SavePointsManager = new SavePointsManager();
        ArrowForPlayerManager = new ArrowForPlayerManager();
        QuestManager = FindAnyObjectByType<QuestManager>();
        saveGame = new SaveGame();

        CoinManager.Init();
        UIManager.Init();
        LevelChangeObserver.Init();
        playerController.Init();
        ArrowForPlayerManager.Init();
        QuestManager.Init();
    }

    public void SavePlayerPosLikeaPause(bool p)
    {
        if (p)
        {
            IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(1000, 1000, 1000);
            IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
            isPause = true;
        }
        else
        {
            IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(0, 0, 0);
            IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
            isPause = false;
        }
        //Time.timeScale = p ? 0.01f : 1;
    }
}
