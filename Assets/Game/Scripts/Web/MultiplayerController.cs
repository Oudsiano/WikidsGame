using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UI;

public class MultiplayerController : MonoBehaviour
{
    public static MultiplayerController Instance;

    [FormerlySerializedAs("otherPlayer")] [SerializeField] private OtherPlayer _otherPlayerPrefab;

    private UIManager _uiManager;
    private OtherPlayer _otherPlayer;
    private string _otherPlayerId;
    private bool _isSpawned;
    
    
    
    public void Construct()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void MoveOtherPlayerAlongPath(Vector3 position)
    {
        if (_otherPlayer != null)
            // _otherPlayer.OtherPlayerController.SetTrajectory(trajectory);
            _otherPlayer.OtherPlayerController.Move(position);
    }

    public void SetOtherPlayerProfile(string username, string avatarUrl)
    {
        if (_otherPlayerPrefab != null)
        {
            // future: update UI, skin, etc.
            Debug.Log($"Set other player profile: {username} ({avatarUrl})");
        }
    }
    
    public void SpawnOtherPlayer(string playerId, Vector3 position)
    {
        Debug.Log($"[MultiplayerController] SpawnOtherPlayer called with playerId: {playerId}, MyLocalPlayerId: {SocketManager.MyLocalPlayerId}, _otherPlayerId: {_otherPlayerId}, _otherPlayer == null: {_otherPlayer == null}");
        
        if (playerId == SocketManager.MyLocalPlayerId)
        {
            Debug.Log($"[MultiplayerController] Ignoring spawn for local playerId: {playerId}");
        }
        else
        {
            if (_otherPlayerPrefab != null)
            {
                // if (_otherPlayerId == playerId)
                // {
                //     Debug.LogWarning($"[MultiplayerController] OtherPlayer already exists for playerId: {playerId}");
                //     return;
                // }
                // else
                // {
                //     // Уничтожаем старого игрока (например, при переподключении противника)
                //     Destroy(_otherPlayer.gameObject);
                //     _otherPlayer = null;
                //     _otherPlayerId = null;
                // }

                if (_isSpawned)
                {
                    Debug.LogWarning("[MultiplayerController] Other player is already spawned. Ignoring new spawn request.");
                    return;
                }
            
                Debug.Log("Start spawning other player...");
                Debug.Log($"[MultiplayerController] Spawning OtherPlayer for playerId: {playerId} at {position}");
                _otherPlayer = Instantiate(_otherPlayerPrefab, position, Quaternion.identity);
                LoadModularCharacter(_otherPlayer).Forget();
                _otherPlayerId= playerId;
                _otherPlayer.Construct(_uiManager);
                _isSpawned = true;  
                Debug.Log($"👤 Second player spawned at: {position}");
            }
        }
    }

    public  void DestroyOtherPlayer()
    {
        Debug.Log("[MultiplayerController] Destroying other player");
        _isSpawned = false;
        Destroy(_otherPlayer.gameObject);
    }
    
    public void UpdateOtherPlayerHealth(float health)
    {
        if (_otherPlayer != null)
        {
            Debug.Log($"[MultiplayerController] Updating other player health to {health}");
            _otherPlayer.OtherPlayerHealth.UpdateHealth(health);
        }

        if (health<=0)
        {
            _isSpawned = false;   
            Destroy(_otherPlayer.gameObject);
        }
    }
    
    
    private async UniTaskVoid LoadModularCharacter(OtherPlayer player)
    {
        Debug.Log($"[LoadModularCharacter] Запуск для: {player}, isNull = {player == null}");
        
        await UniTask.NextFrame();
        
        if (player == null)
        {
            Debug.LogError("❌ LoadModularCharacter: player = null");
            return;
        }

        if (player.gameObject == null)
        {
            Debug.LogError("❌ LoadModularCharacter: player.gameObject = null");
            return;
        }

        if (player.IfModularCharacterCreated) return;

        var handle = Addressables.LoadAssetAsync<GameObject>("PlayerModel");
        var modularCharacter = await handle.Task;
        
        
        if (modularCharacter == null)
        {
            Debug.LogError("❌ Не удалось загрузить PlayerModel");
            return;
        }
        
        if (player.transform == null)
        {
            Debug.LogError("❌ Player.transform = null, не можем инстанциировать модель");
            return;
        }

        Debug.Log("Начинаем интантиирование");
        var go = Instantiate(modularCharacter, player.transform);
        Debug.Log($"✅ Instantiated ModularCharacter GO: {go.name}, active: {go.activeInHierarchy}, scale: {go.transform.localScale}");
        
        foreach (var r in go.GetComponentsInChildren<MeshRenderer>(true))
        {
            Debug.Log($"[AFTER Instantiate] Renderer: {r.gameObject.name}, enabled: {r.enabled}, active: {r.gameObject.activeInHierarchy}, scale: {r.transform.localScale}");
        }

        var animator = player.GetComponent<Animator>();
        animator.Rebind(); // иногда помогает при странностях
        animator.Play("Locomotion"); // или имя твоей начальной анимации
        animator.Update(0f); // форс-применение текущей анимации
        player.IsCreatedModularCharacter();
        Debug.Log("✅ Модульный персонаж успешно создан для игрока");
    }
}
