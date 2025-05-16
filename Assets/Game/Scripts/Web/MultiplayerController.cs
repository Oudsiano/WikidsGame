using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class MultiplayerController : MonoBehaviour
{
    public static MultiplayerController Instance;

    [FormerlySerializedAs("otherPlayer")] [SerializeField] private OtherPlayerController _otherPlayerPrefab;

    
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

    public void MoveOtherPlayerAlongPath(List<Vector3> trajectory)
    {
        if (_otherPlayerPrefab != null)
            _otherPlayerPrefab.SetTrajectory(trajectory);
    }

    public void SetOtherPlayerProfile(string username, string avatarUrl)
    {
        if (_otherPlayerPrefab != null)
        {
            // future: update UI, skin, etc.
            Debug.Log($"Set other player profile: {username} ({avatarUrl})");
        }
    }
    
    public void SpawnOtherPlayer(Vector3 position)
    {
        if (_otherPlayerPrefab != null)
        {
            var other = Instantiate(_otherPlayerPrefab, new Vector3(195, -24.00106f,38.42f), Quaternion.identity);
            LoadModularCharacter(other).Forget();
            Debug.Log($"👤 Second player spawned at: {position}");
        }
    }
    
    private async UniTaskVoid LoadModularCharacter(OtherPlayerController player)
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
        Instantiate(modularCharacter, player.transform);
        var animator = player.GetComponent<Animator>();
        animator.Rebind(); // иногда помогает при странностях
        animator.Play("Locomotion"); // или имя твоей начальной анимации
        animator.Update(0f); // форс-применение текущей анимации
        player.IsCreatedModularCharacter();
        Debug.Log("✅ Модульный персонаж успешно создан для игрока");
    }
}
