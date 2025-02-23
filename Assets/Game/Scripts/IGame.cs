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
using UnityEngine.Serialization;

public class IGame : MonoBehaviour // TODO OVERLOAD CLASS NEED TO FULL REFACTOR FOR COMPOSE
{
    private static IGame instance; // TODO delete

    public DataPlayer dataPlayer;
    public GameAPI gameAPI;
    public SaveGame saveGame;

    public PlayerController playerController;
    public ArrowForPlayerManager ArrowForPlayerManager;
    public NPCManagment NPCManagment;
    public CursorManager CursorManager;
    
    private FastTestsManager _fastsTestsManager;
    [FormerlySerializedAs("UIManager")] [SerializeField] public UIManager _uiManager; // TODO change
    [FormerlySerializedAs("CoinManager")] [SerializeField] public CoinManager _coinManager;
    [FormerlySerializedAs("BottleManager")] [SerializeField] public BottleManager _bottleManager;

    [FormerlySerializedAs("weaponArmorManager")] [SerializeField]
    private WeaponArmorManager _weaponArmorManager; // TODO CHANGE

    private SavePointsManager _savePointsManager;
    private LevelChangeObserver _levelChangeObserver;
    private QuestManager _questManager;
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
        BottleManager bottleManager, WeaponArmorManager weaponArmorManager)
    {
        Debug.Log("Construct iagem");
        _player = player;
        this.gameAPI = gameAPI;
        this.dataPlayer = dataPlayer;
        this.saveGame = saveGame;
        this.playerController = playerController;
        _levelChangeObserver = levelChangeObserver;
        _savePointsManager = savePointsManager;
        ArrowForPlayerManager = arrowForPlayerManager;
        QuestManager = questManager;
        NPCManagment = npcManagment;
        _fastsTestsManager = fastTestsManager;
        CursorManager = cursorManager;
        _uiManager = uiManager;
        _coinManager = coinManager;
        _bottleManager = bottleManager;
        _weaponArmorManager = weaponArmorManager;
        
        levelChangeObserver.Construct(_savePointsManager, this.dataPlayer, _uiManager, _player, this.gameAPI);
        ArrowForPlayerManager.Construct();
        questManager.Construct(this.dataPlayer, _uiManager, _levelChangeObserver);
        NPCManagment.Construct(this.dataPlayer);
        _fastsTestsManager.Construct(this.dataPlayer, _uiManager);
        _coinManager.Construct();
    }

    public FastTestsManager FastTestsManager => _fastsTestsManager;
    
    public static IGame Instance // TODO CHANGE
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
        get => _levelChangeObserver;
        set => _levelChangeObserver = value;
    }

    public WeaponArmorManager WeaponArmorManager
    {
        get => _weaponArmorManager;
        set => _weaponArmorManager = value;
    }

    public QuestManager QuestManager
    {
        get => _questManager;
        set => _questManager = value;
    }

    public void SavePlayerPosLikeaPause(bool p)
    {
        if (p)
        {
            playerController.ModularCharacter.transform.localPosition = new Vector3(1000, 1000, 1000);
            playerController.ModularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            playerController.ModularCharacter.transform.localPosition = new Vector3(0, 0, 0);
            playerController.ModularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //Time.timeScale = p ? 0.01f : 1;
    }
}