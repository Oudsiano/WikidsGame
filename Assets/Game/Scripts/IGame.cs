using System.Collections;
using System.Collections.Generic;
using AINavigation;
using Combat;
using Core;
using Core.Player;
using Core.Quests;
using Data;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;

public class IGame : MonoBehaviour // TODO OVERLOAD CLASS NEED TO FULL REFACTOR FOR COMPOSE
{
    private static IGame instance;

    public DataPlayer dataPlayer;
    public GameAPI gameAPI;
    public SaveGame saveGame;

    public PlayerController playerController;
    public SavePointsManager SavePointsManager;
    public ArrowForPlayerManager ArrowForPlayerManager;
    public NPCManagment NPCManagment;
    public FastTestsManager FastTestsManager;
    public CursorManager CursorManager;

    [SerializeField] public UIManager UIManager;
    [SerializeField] public CoinManager CoinManager;
    [SerializeField] public BottleManager BottleManager;
    [SerializeField] private WeaponArmorManager weaponArmorManager; // TODO CHANGE

    private LevelChangeObserver levelChangeObserver;
    private QuestManager questManager;
    private PlayerArmorManager _playerArmorManager;
    private WeaponPanelUI _weaponPanelUI;
    private MainPlayer _player;
    
    public void Construct
    (
        MainPlayer player, GameAPI gameAPI, DataPlayer dataPlayer, SaveGame saveGame,
        PlayerController playerController, LevelChangeObserver levelChangeObserver,
        SavePointsManager savePointsManager, ArrowForPlayerManager arrowForPlayerManager,
        QuestManager questManager, NPCManagment npcManagment, FastTestsManager fastTestsManager,
        CursorManager cursorManager, UIManager uiManager, CoinManager coinManager,
        BottleManager bottleManager, WeaponArmorManager weaponArmorManager, 
        PlayerArmorManager playerArmorManager, WeaponPanelUI weaponPanelUI)
    {
        _player = player;
        this.gameAPI = gameAPI;
        this.dataPlayer = dataPlayer;
        this.saveGame = saveGame;
        this.playerController = playerController;
        this.levelChangeObserver = levelChangeObserver;
        SavePointsManager = savePointsManager;
        ArrowForPlayerManager = arrowForPlayerManager;
        QuestManager = questManager;
        NPCManagment = npcManagment;
        FastTestsManager = fastTestsManager;
        CursorManager = cursorManager;
        UIManager = uiManager;
        CoinManager = coinManager;
        BottleManager = bottleManager;
        this.weaponArmorManager = weaponArmorManager;
        _playerArmorManager = playerArmorManager;
        _weaponPanelUI = weaponPanelUI;
        
        playerController.Construct(_playerArmorManager, _weaponPanelUI, this.saveGame); 
        levelChangeObserver.Construct(SavePointsManager, this.dataPlayer, UIManager, _player, this.gameAPI);
        ArrowForPlayerManager.Construct();
        questManager.Construct(this.dataPlayer, UIManager, this.levelChangeObserver);
        NPCManagment.Construct(this.dataPlayer);
        FastTestsManager.Construct(this.dataPlayer, UIManager); 
        UIManager.Construct(); // TODO construct // остановился здесь
        CoinManager.Construct(); // TODO construct
    }

    // private void Init()
    // {
    //     dataPlayer = FindObjectOfType<DataPlayer>();
    //     gameAPI = FindObjectOfType<GameAPI>();
    //     playerController = FindObjectOfType<PlayerController>();
    //     BottleManager = FindAnyObjectByType<BottleManager>();
    //     SavePointsManager = new SavePointsManager();
    //     ArrowForPlayerManager = new ArrowForPlayerManager();
    //     questManager = FindAnyObjectByType<QuestManager>();
    //     NPCManagment = gameObject.AddComponent<NPCManagment>();
    //     FastTestsManager = new FastTestsManager();
    //     CursorManager = FindAnyObjectByType<CursorManager>();
    //
    //     saveGame = new SaveGame();
    //
    //     NPCManagment.Init();
    //     questManager.Init();
    //     CoinManager.Init();
    //     UIManager.Init();
    //     playerController.Construct(); // *
    //     ArrowForPlayerManager.Init();
    //     FastTestsManager.Init();
    // }

    public static IGame Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<IGame>();
                /*if (instance==null)
                {
                    SceneComponent sc = FindObjectOfType<SceneComponent>();
                    if (sc!=null)
                    {
                        Instantiate(sc.TheCore);
                        instance = FindObjectOfType<IGame>();
                    }
                    else
                    {
                        Debug.LogError("Not added Core prefab");
                    }
                }
                */
                Debug.Log("ЗДЕСЬ ДОЛЖЕН БЫЛ СЛУЧИТЬСЯ ИНИТ!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                //instance.Init();
            }

            return instance;
        }
        set => instance = value;
    }

    public LevelChangeObserver LevelChangeObserver
    {
        get
        {
            if (levelChangeObserver == null)
            {
                levelChangeObserver = FindAnyObjectByType<LevelChangeObserver>();
                // levelChangeObserver.Init(); // 
            }

            return levelChangeObserver;
        }
        set => levelChangeObserver = value;
    }

    public WeaponArmorManager WeaponArmorManager
    {
        get
        {
            if (weaponArmorManager == null)
            {
                weaponArmorManager = FindAnyObjectByType<WeaponArmorManager>();
            }

            return weaponArmorManager;
        }
        set => weaponArmorManager = value;
    }

    public QuestManager QuestManager
    {
        get
        {
            if (questManager == null)
            {
                questManager = FindAnyObjectByType<QuestManager>();
                questManager.Init();
            }

            return questManager;
        }
        set => questManager = value;
    }


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

    public void SavePlayerPosLikeaPause(bool p)
    {
        if (p)
        {
            IGame.Instance.playerController.ModularCharacter.transform.localPosition = new Vector3(1000, 1000, 1000);
            IGame.Instance.playerController.ModularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            IGame.Instance.playerController.ModularCharacter.transform.localPosition = new Vector3(0, 0, 0);
            IGame.Instance.playerController.ModularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //Time.timeScale = p ? 0.01f : 1;
    }
}