using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Movement;
using UnityEngine.AddressableAssets;

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
    private Dictionary<string, GameObject> otherPlayers = new();
    private string _myPlayerId;
    private string _pendingPlayerId;
    private bool _waitingForPlayerMover = true;

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
                playerMover.SetPlayerId(_pendingPlayerId);
                _myPlayerId = _pendingPlayerId;
                _pendingPlayerId = null;
                _waitingForPlayerMover = false;

                // Отправим текущую позицию
                SendMyPosition();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var other = Instantiate(otherPlayerPrefab, new Vector3(189, -23, 44), Quaternion.identity);
            LoadModularCharacter(other).Forget();
        }
    }

    public void OnYouAre(string playerId)
    {
        Debug.Log("🔹 Это наш ID: " + playerId);
        _myPlayerId = playerId;
    }

    public void OnPlayerData(string json)
    {
        // Проверка: это запрос позиции?
        if (json.Contains("requestPosition"))
        {
            var req = JsonUtility.FromJson<PlayerRequest>(json);
            if (req.requestPosition == _myPlayerId)
            {
                Debug.Log($"📨 Игрок {req.id} просит нашу позицию");
                SendMyPosition();
            }
            return;
        }

        var data = JsonUtility.FromJson<PlayerNetworkPositionData>(json);
        if (data.id == _myPlayerId) return;

        // Создание нового игрока
        if (!otherPlayers.TryGetValue(data.id, out var existingPlayer))
        {
            var other = Instantiate(otherPlayerPrefab, new Vector3(data.x, data.y, data.z), Quaternion.identity);
            LoadModularCharacter(other).Forget();
            otherPlayers[data.id] = other.gameObject;
            existingPlayer = other.gameObject;

            Debug.Log($"🆕 Создан другой игрок: {data.id}, позиция: ({data.x}, {data.y}, {data.z})");
        }

        // Обновление позиции
        var controller = existingPlayer.GetComponent<OtherPlayerController>();
        if (controller != null)
        {
            controller.SetPosition(new Vector3(data.x, data.y, data.z));
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
            }
        }
    }

    private async UniTaskVoid LoadModularCharacter(OtherPlayerController player)
    {
        if (player.IfModularCharacterCreated) return;

        var handle = Addressables.LoadAssetAsync<GameObject>("PlayerModel");
        var modularCharacter = await handle.Task;
        if (modularCharacter == null)
        {
            Debug.LogError("❌ Не удалось загрузить PlayerModel");
            return;
        }

        Instantiate(modularCharacter, player.transform);
        player.IsCreatedModularCharacter();
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

