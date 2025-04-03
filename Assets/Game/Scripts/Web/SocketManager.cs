using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Movement;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class SocketManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPlayerData(string data);

    [DllImport("__Internal")]
    private static extern void DisconnectSocket();

    private List<string> connectedPlayers = new List<string>();
    [SerializeField] private OtherPlayerController otherPlayerPrefab;
    private Dictionary<string, OtherPlayerController> otherPlayers = new();
    
    private Dictionary<string, PlayerNetworkPositionData> playerDataById = new();
    private string _myPlayerId;
    private string _pendingPlayerId;
    private bool _waitingForPlayerMover = true;
    private Animator _animator;

    [System.Serializable]
    private class PlayerListWrapper
    {
        public string[] playerIds;
    }

    [System.Serializable]
    private class PlayerRequest
    {
        public string id;
        public string requestPosition;
    }

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ConnectToSocket();
#endif
    }

    public void SendData(string data)
    {
        Debug.Log($"📤 Отправка на сервер: {data}");
#if UNITY_WEBGL && !UNITY_EDITOR
        SendPlayerData(data);
#else
        Debug.Log("Sending data: " + data);
#endif
    }

    public void OnConnected()
    {
        Debug.Log("✅ Вы подключились к серверу!");
    }

    private void Update()
    {
        if (_waitingForPlayerMover && !string.IsNullOrEmpty(_pendingPlayerId))
        {
            var playerMover = FindObjectOfType<PlayerMover>();
            if (playerMover != null)
            {
                Debug.Log("🔍 Найден PlayerMover, устанавливаем ID");
                playerMover.SetPlayerId(_pendingPlayerId);
                _myPlayerId = _pendingPlayerId;
                _pendingPlayerId = null;
                _waitingForPlayerMover = false;

                // Отправим текущую позицию
                SendMyPosition();
            }
            else
            {
                Debug.LogWarning("⚠️ PlayerMover не найден!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var other = Instantiate(otherPlayerPrefab, new Vector3(189, -23, 44), Quaternion.identity);
            LoadModularCharacter(other).Forget();
            _animator = other.GetComponent<Animator>();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            _animator.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _animator.enabled = true;
        }
    }

    public void OnYouAre(string playerId)
    {
        Debug.Log("🔹 Это наш ID: " + playerId);
        _pendingPlayerId = playerId;
        _waitingForPlayerMover = true;
        
        SendMyPosition();
    }

    public void OnPlayerData(string json)
    {
        Debug.Log($"📩 Получен playerData: {json}");

        if (string.IsNullOrEmpty(_myPlayerId))
        {
            Debug.LogWarning("⚠️ _myPlayerId ещё не установлен, пропускаем обработку playerData");
            return;
        }

        try
        {
            // 🛠️ Фикс двойной сериализации (строка внутри строки)
            if (json.StartsWith("\""))
            {
                json = JsonConvert.DeserializeObject<string>(json);
                Debug.Log($"🛠️ Распакованный JSON: {json}");
            }

            var jObject = JObject.Parse(json);

            if (jObject["requestPosition"] != null)
            {
                var req = jObject.ToObject<PlayerRequest>();
                if (req.requestPosition == _myPlayerId)
                {
                    Debug.Log($"📨 Игрок {req.id} просит нашу позицию");
                    SendMyPosition();
                }
                else
                {
                    Debug.Log($"📭 Запрос позиции, но не к нам: {req.requestPosition}");
                }
                return;
            }

            var data = jObject.ToObject<PlayerNetworkPositionData>();
            if (data.id == _myPlayerId)
            {
                Debug.Log("⏭️ Пропускаем собственную позицию");
                return;
            }

            Debug.Log($"🧍 Получены координаты игрока {data.id}");

            if (otherPlayers.TryGetValue(data.id, out var existingPlayer))
            {
                existingPlayer.SetPosition(new Vector3(data.x, data.y, data.z));
                Debug.Log($"📍 Обновлена позиция игрока {data.id} на x:{data.x}, y:{data.y}, z:{data.z}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Игрок с id {data.id} ещё не создан, позиция сохранена в ожидании спавна");
                playerDataById[data.id] = data;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Ошибка при обработке JSON с Newtonsoft: {ex.Message}\nJSON: {json}");
        }
    }

    public void SpawnOtherPlayers(Vector3 position, Quaternion rotation)
    {
        Debug.Log("🚀 Спавним других игроков из playerDataById");
        
        foreach (var playerData in playerDataById)
        {
            Debug.Log($"id={playerData.Value.id}, x={playerData.Value.x },y={playerData.Value.y},z={playerData.Value.z}");
        }

        if (playerDataById.Count <= 0)
        {
            Debug.Log("Других игроков нет");
            return;
        }
        foreach (var kvp in playerDataById)
        {
            string id = kvp.Key;
            var data = kvp.Value;

            if (id == _myPlayerId) continue; // не спавним сами себя
            Debug.Log($"🆕 Инстанциируем игрока {id} на позиции {data.x}, {data.y}, {data.z}");
            var other = Instantiate(otherPlayerPrefab, position, rotation);
            LoadModularCharacter(other).Forget();
            Animator otherAnimator = other.GetComponent<Animator>();
            otherAnimator.enabled = false;
            otherAnimator.enabled = true;
            otherPlayers[id] = other;
            Debug.Log("Игрок создан!");
        }
    }



    public void OnPlayerConnected(string playerId)
    {
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            Debug.Log($"🔗 Игрок {playerId} подключился!");

            // Попросим его отправить свою позицию
            var request = new PlayerRequest { id = _myPlayerId, requestPosition = playerId };
            SendData(JsonUtility.ToJson(request));
        }
    }

    public void OnExistingPlayers(string playerIdsJson)
    {
        Debug.Log("📥 OnExistingPlayers вызван!");

        var wrapper = JsonUtility.FromJson<PlayerListWrapper>(playerIdsJson);
        Debug.Log($"👥 Всего игроков получено: {wrapper.playerIds.Length}");

        foreach (var id in wrapper.playerIds)
        {
            if (!otherPlayers.ContainsKey(id))
            {
                connectedPlayers.Add(id);
                Debug.Log($"📡 Запрашиваем позицию у игрока: {id}");
                var request = new PlayerRequest { id = _myPlayerId, requestPosition = id };
                SendData(JsonUtility.ToJson(request));
            }
        }
    }

    public void OnPlayerDisconnected(string playerId)
    {
        if (connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Remove(playerId);
            Debug.Log($"❌ Игрок {playerId} отключился");

            if (otherPlayers.TryGetValue(playerId, out var go))
            {
                Destroy(go);
                otherPlayers.Remove(playerId);
                playerDataById.Remove(playerId);
            }
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


    private void SendMyPosition()
    {
        var mover = FindObjectOfType<PlayerMover>();
        if (mover == null) return;

        var pos = mover.transform.position;
        var data = new PlayerNetworkPositionData
        {
            id = _myPlayerId,
            x = pos.x,
            y = pos.y,
            z = pos.z
        };
        SendData(JsonUtility.ToJson(data));
        Debug.Log($"📤 Отправлена позиция игрока {_myPlayerId}: {pos}");
    }

    public void OnServerFull(string message)
    {
        Debug.LogWarning(message);
    }

    void OnDestroy()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        DisconnectSocket();
#endif
    }
}

[System.Serializable]
public class PlayerNetworkPositionData
{
    public string id;
    public float x, y, z;
}

[Serializable]
public class PlayerRequest
{
    public string id;
    public string requestPosition;
}

