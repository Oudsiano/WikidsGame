using RPG.Controller;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGame : MonoBehaviour
{
    private static IGame instance;

    public DataPlayer dataPlayer;
    public GameAPI gameAPI;
    public SaveGame saveGame;

    public PlayerController playerController;
    private LevelChangeObserver levelChangeObserver;
    public SavePointsManager SavePointsManager;
    public ArrowForPlayerManager ArrowForPlayerManager;
    private QuestManager questManager;
    public NPCManagment NPCManagment;
    public FastTestsManager FastTestsManager;
    public CursorManager CursorManager;

    [SerializeField] public UIManager UIManager;
    [SerializeField] public CoinManager CoinManager;
    [SerializeField] public BottleManager BottleManager;
    [SerializeField] private WeaponArmorManager weaponArmorManager;

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
                instance.Init();
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
                levelChangeObserver.Init();
            }
            return levelChangeObserver;
        }
        set => levelChangeObserver = value;
    }

    public WeaponArmorManager WeaponArmorManager
    {
        get
        {
            if (weaponArmorManager==null)
            {
                weaponArmorManager = FindAnyObjectByType<WeaponArmorManager>();
            }
            return weaponArmorManager;
        }
        set => weaponArmorManager = value;
    }

    public QuestManager QuestManager { 
        get 
        {
            if (questManager == null)
            {
                questManager = FindAnyObjectByType<QuestManager>();
                questManager.Init();
            }
            return questManager; 
        }
        set => questManager = value; }
    

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
        dataPlayer = FindObjectOfType<DataPlayer>();
        gameAPI = FindObjectOfType<GameAPI>();
        playerController = FindObjectOfType<PlayerController>();
        BottleManager = FindAnyObjectByType<BottleManager>();
        SavePointsManager = new SavePointsManager();
        ArrowForPlayerManager = new ArrowForPlayerManager();
        questManager = FindAnyObjectByType<QuestManager>();
        NPCManagment = gameObject.AddComponent<NPCManagment>();
        FastTestsManager = new FastTestsManager();
        CursorManager = FindAnyObjectByType<CursorManager>();

        saveGame = new SaveGame();

        NPCManagment.Init();
        questManager.Init();
        CoinManager.Init();
        UIManager.Init();
        playerController.Init();
        ArrowForPlayerManager.Init();
        FastTestsManager.init();
    }

    public void SavePlayerPosLikeaPause(bool p)
    {
        if (p)
        {
            IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(1000, 1000, 1000);
            IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(0, 0, 0);
            IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //Time.timeScale = p ? 0.01f : 1;
    }
}
