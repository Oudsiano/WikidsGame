using System.Collections;
using System.Collections.Generic;
using AINavigation;
using UnityEngine;
using Combat;
using Core;
using Core.Camera;
using Core.Player;
using Core.Quests;
using Healths;
using Movement;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyFabric<T> : MonoBehaviour where T:MonoBehaviour
{
    [SerializeField] private List<T> _enemies;
    
    private PlayerController _playerController;
    private MainPlayer _mainPlayer;
    private IGame _igame;
    private FastTestsManager _fastTestsManager;
    private QuestManager _questManager;
    private CoinManager _coinManager;
    private BottleManager _bottleManager;

    
    public void Init(PlayerController playerController, MainPlayer player, IGame igame, FastTestsManager fastTestsManager,
        QuestManager questManager, CoinManager coinManager, BottleManager bottleManager)
    {
        _playerController = playerController;
        _igame = igame;
        _fastTestsManager = fastTestsManager;
        _questManager = questManager;
        _coinManager = coinManager;
        _bottleManager = bottleManager;
        _mainPlayer = player;
    }
    
    
    public void SpawnEnemy(int index,Transform transform)
    {
        CreateEnemy(_enemies[index], transform);
    }

    private void CreateEnemy(MonoBehaviour enemy, Transform transform)
    {
        var createdEnemy = Instantiate(enemy, transform.position, transform.rotation);
        AIController archerController = createdEnemy.GetComponent<AIController>();
        archerController.Construct(_playerController, _mainPlayer, _igame, _fastTestsManager, _questManager, _coinManager, _bottleManager);
    }
}
