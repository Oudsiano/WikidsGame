using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SocketManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPlayerData(string data);

    [DllImport("__Internal")]
    private static extern void DisconnectSocket();

    private List<string> connectedPlayers = new List<string>();

    [System.Serializable]
    private class PlayerListWrapper
    {
        public string[] playerIds;
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
#endif
    }

    public void OnConnected()
    {
        Debug.Log("Вы подключились к серверу!");
    }

    public void OnPlayerConnected(string playerId)
    {
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            Debug.Log($"Игрок {playerId} подключился!");
        }
    }

    public void OnExistingPlayers(string playerIdsJson)
    {
        var wrapper = JsonUtility.FromJson<PlayerListWrapper>(playerIdsJson);
        foreach (var id in wrapper.playerIds)
        {
            if (!connectedPlayers.Contains(id))
            {
                connectedPlayers.Add(id);
                Debug.Log($"Обнаружен существующий игрок: {id}");
            }
        }
    }

    public void OnPlayerDisconnected(string playerId)
    {
        if (connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Remove(playerId);
            Debug.Log($"Игрок {playerId} отключился!");
        }
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