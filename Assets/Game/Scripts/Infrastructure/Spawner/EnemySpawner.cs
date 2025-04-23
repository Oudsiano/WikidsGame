using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
using Core.NPC;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyFabric<Archer> _archerFabric;
    [SerializeField] private EnemyFabric<Sword> _swordFabric;
    [SerializeField] private EnemyFabric<Mace> _maceFabric;
    
    [SerializeField] private List<ArcherSpawnPoint> _archerSpawnPoints;

    [SerializeField] private List<EnemySoldierMace1SpawnPoint> _enemySoldierMace1SpawnPoints;
    [SerializeField] private List<EnemySoldierMace2SpawnPoint> _enemySoldierMace2SpawnPoints;
    [SerializeField] private List<EnemySoldierMace3SpawnPoint> _enemySoldierMace3SpawnPoints;

    [SerializeField] private List<EnemySoldierSword1SpawnPoint> _enemySoldierSword1SpawnPoints;
    [SerializeField] private List<EnemySoldierSword2SpawnPoint> _enemySoldierSword2SpawnPoints;
    [SerializeField] private List<EnemySoldierSword3SpawnPoint> _enemySoldierSword3SpawnPoints;
    
    [SerializeField] private List<BossNPC> _bossNPCs;
    
    private readonly Dictionary<SpawnPoint, GameObject> _spawnedEnemies = new Dictionary<SpawnPoint, GameObject>();


    public void Construct(PlayerController playerController, MainPlayer player, IGame igame,
        FastTestsManager fastTestsManager,
        QuestManager questManager, CoinManager coinManager, BottleManager bottleManager)
    {
        _archerFabric.Init(playerController, player, igame, fastTestsManager, questManager, coinManager, bottleManager);
        _swordFabric.Init(playerController, player, igame, fastTestsManager, questManager, coinManager, bottleManager);
        _maceFabric.Init(playerController, player, igame, fastTestsManager, questManager, coinManager, bottleManager);
        
        SpawnEnemies(1,_archerSpawnPoints, _archerFabric);
        
        SpawnEnemies(0,_enemySoldierMace1SpawnPoints, _maceFabric);
        SpawnEnemies(1,_enemySoldierMace2SpawnPoints, _maceFabric);
        SpawnEnemies(2,_enemySoldierMace3SpawnPoints, _maceFabric);
        
        SpawnEnemies(0,_enemySoldierSword1SpawnPoints, _swordFabric);
        SpawnEnemies(1,_enemySoldierSword2SpawnPoints, _swordFabric);
        SpawnEnemies(2,_enemySoldierSword3SpawnPoints, _swordFabric);

        foreach (var bossNPC in _bossNPCs)
        {
            List<GameObject> enemies = new List<GameObject>();
            
            foreach (var spawnPoint in bossNPC.AssociatedSpawnPoints)
            {
                if (spawnPoint == null)
                {
                    Debug.LogWarning("Точка спавна равна null для BossNPC!");
                    continue;
                }

                if (_spawnedEnemies.TryGetValue(spawnPoint, out GameObject enemy) && enemy != null)
                {
                    enemies.Add(enemy);
                }
            }
            
            bossNPC.SetEnemies(enemies);
            bossNPC.Construct();
        }
    }


    public void SpawnEnemies<T, TSpawnPoint>(int index,List<TSpawnPoint> enemies, EnemyFabric<T> _enemyFabric) where T : MonoBehaviour where TSpawnPoint :SpawnPoint
    {
        if (enemies.Count == 0)
        {
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            _enemyFabric.SpawnEnemy(index, enemies[i].transform);
        }
    }
}