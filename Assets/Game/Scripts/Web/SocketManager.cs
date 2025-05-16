using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

public class SocketManager : MonoBehaviour
{
    #region Native Methods
    [DllImport("__Internal")] private static extern void InitializeSocketWithCallbacks();
    [DllImport("__Internal")] private static extern void RegisterUnityCallback(string componentName, string methodName, Action<string> callback);
    [DllImport("__Internal")] private static extern void SendWebGLMessage(string componentName, string methodName, string jsonData);
    [DllImport("__Internal")] private static extern int IsSocketInitialized();
    [DllImport("__Internal")] private static extern void DestroySocket();
    [DllImport("__Internal")] private static extern void TestJSLib();
    
    [DllImport("__Internal")] private static extern void FastJoinToRoom();
    
    [DllImport("__Internal")] private static extern void ConfirmRoom();
    #endregion

    #region Events
    // Общие события
    public static event Action<string> OnError;
  
    // Profile события
    public static event Action<string> OnProfileReceived;
  
    // Room события
    public static event Action<string> OnFastJoinRoomReceived;
    public static event Action<string> OnLeaveRoomReceived;
    public static event Action<string> OnStateConfirmationSent;
    public static event Action<string> OnRoomStateUpdated;
    public static event Action<string> OnOpponentConfirmationReceived;
    public static event Action<string> OnRoomLeaveTimeout;
  
    // Game события
    public static event Action<string> OnMirrorActionResponse;
    public static event Action<string> OnStateReceived;
    public static event Action<string> OnWinnerSet;
    public static event Action<string> OnSurrenderProcessed;
    public static event Action<string> OnGameStateChanged;
  
    // Chat события
    public static event Action<string> OnChatSent;
    public static event Action<string> OnChatReceived;
  
    // Stats события
    public static event Action<string> OnAllStatsReceived;
    public static event Action<string> OnPvEStatsReceived;
    public static event Action<string> OnPvPStatsReceived;
  
    // Settings события
    public static event Action<string> OnGlobalSettingsReceived;
    #endregion

    #region Serializable Models
    [Serializable] public class ProfileRequest { public string userId; }
    [Serializable] public class ProfileResponse { public string id; public string username; public string avatarUrl; }
    [Serializable] public class ErrorResponse { public string error; }

    [Serializable] public class MirrorBaseAction { public string method; }
    [Serializable] public class Vector3Wrapper { public float x, y, z; }

    [Serializable] public class MoveParams { public List<Vector3Wrapper> trajectory; }
    [Serializable] public class MirrorMoveAction { public string method; public MoveParams @params; }

    [Serializable] public class SpawnParams { public Vector3Wrapper position; }
    [Serializable] public class SpawnActionWrapper { public string method; public SpawnParams @params; }
    
    [Serializable]
    public class PlayerInfo
    {
        public string _id;
        public string username;
        public string avatarUrl;
    }

    [Serializable]
    public class RoomStateWrapper
    {
        public string status;
        public PlayerInfo[] players;
    }
    #endregion

    
    public void Construct()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterCallbacks();
        InitializeSocketWithCallbacks();
        TestJSLib();
        FastJoinToRoom();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void RegisterCallbacks()
    {
        // Profile
        RegisterUnityCallback("profile", "onProfileReceived", OnProfileReceivedCallback);
        
        // Room
         RegisterUnityCallback("room", "onFastJoinRoomReceived", OnFastJoinRoomReceivedCallback);
        // RegisterUnityCallback("room", "onLeaveRoomReceived", OnLeaveRoomReceivedCallback);
        // RegisterUnityCallback("room", "onStateConfirmationSent", OnStateConfirmationSentCallback);
        RegisterUnityCallback("room", "onRoomStateUpdated", OnRoomStateUpdatedCallback);
        RegisterUnityCallback("room", "onOpponentConfirmationReceived", OnOpponentConfirmationReceivedCallback);
        // RegisterUnityCallback("room", "onRoomLeaveTimeout", OnRoomLeaveTimeoutCallback);
        
        // Game
         RegisterUnityCallback("game", "onMirrorActionResponse", OnMirrorActionResponseCallback);
        // RegisterUnityCallback("game", "onStateReceived", OnStateReceivedCallback);
        // RegisterUnityCallback("game", "onWinnerSet", OnWinnerSetCallback);
        // RegisterUnityCallback("game", "onSurrenderProcessed", OnSurrenderProcessedCallback);
        RegisterUnityCallback("game", "onGameStateChanged", OnGameStateChangedCallback);
        
        // Chat
        // RegisterUnityCallback("chat", "onChatSent", OnChatSentCallback);
        // RegisterUnityCallback("chat", "onChatReceived", OnChatReceivedCallback);
        
        // Stats
        // RegisterUnityCallback("stats", "onAllStatsReceived", OnAllStatsReceivedCallback);
        // RegisterUnityCallback("stats", "onPvEStatsReceived", OnPvEStatsReceivedCallback);
        // RegisterUnityCallback("stats", "onPvPStatsReceived", OnPvPStatsReceivedCallback);
        
        // Settings
        // RegisterUnityCallback("settings", "onGlobalSettingsReceived", OnGlobalSettingsReceivedCallback);
    }
#endif

    #region Callback Implementations
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnProfileReceivedCallback(string json)
    {
        if (json.Contains("error")) OnError?.Invoke(json);
        else OnProfileReceived?.Invoke(json);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnGameStateChangedCallback(string json)
    {
        if (json.Contains("error")) { OnError?.Invoke(json); return; }

        var baseAction = JsonUtility.FromJson<MirrorBaseAction>(json);

        switch (baseAction.method)
        {
            case "move": HandleMove(json); break;
            case "spawn": HandleSpawn(json); break;
            default:
                Debug.LogWarning($"Unknown mirrorAction: {baseAction.method}");
                break;
        }

        OnGameStateChanged?.Invoke(json);
    }
    #endregion
    
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnMirrorActionResponseCallback(string json)
    {
        if (json.Contains("error"))
        {
            OnError?.Invoke(json);
            return;
        }

        var baseAction = JsonUtility.FromJson<MirrorBaseAction>(json);

        switch (baseAction.method)
        {
            case "move": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - MOVE "); break;
            case "spawn": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - spawn "); break;
            default:
                Debug.LogWarning($"Unknown mirrorAction (response): {baseAction.method}");
                break;
        }

        OnMirrorActionResponse?.Invoke(json);
    }
    
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnFastJoinRoomReceivedCallback(string json)
    {
        Debug.Log("[SOCKETMANAGER] OnFastJoinRoomReceivedCallback");

        //  Подтверждаем участие в комнате
         // ConfirmRoom();
    }
    
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnOpponentConfirmationReceivedCallback(string json)
    {
        Debug.Log("[SOCKETMANAGER] Opponent confirmed. Starting game...");
        
    }
    
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnRoomStateUpdatedCallback(string json)
    {
        Debug.Log($"[SOCKETMANAGER] Room state updated: {json}");

        try
        {
            // Десериализуем игроков из JSON
            var state = JsonUtility.FromJson<RoomStateWrapper>(json);
        
            if (state.players != null && state.players.Length == 2)
            {
                Debug.Log("[SOCKETMANAGER] Two players joined. Sending confirm...");
                ConfirmRoom();
            }
            else
            {
                Debug.Log($"[SOCKETMANAGER] Waiting for more players... Current: {state.players?.Length ?? 0}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse room state: " + e.Message);
        }

        OnRoomStateUpdated?.Invoke(json);
    }


    #region Action Handlers
    private static void HandleMove(string json)
    {
        var action = JsonUtility.FromJson<MirrorMoveAction>(json);
        if (action.@params?.trajectory == null)
        {
            Debug.LogWarning("Empty trajectory");
            return;
        }

        var path = new List<Vector3>();
        foreach (var point in action.@params.trajectory)
            path.Add(new Vector3(point.x, point.y, point.z));

        MultiplayerController.Instance?.MoveOtherPlayerAlongPath(path);
    }

    private static void HandleSpawn(string json)
    {
        try
        {
            var spawn = JsonUtility.FromJson<SpawnActionWrapper>(json);
            var pos = spawn.@params.position;
            MultiplayerController.Instance?.SpawnOtherPlayer(new Vector3(pos.x, pos.y, pos.z));
        }
        catch (Exception e)
        {
            Debug.LogError("Spawn parse error: " + e.Message);
        }
    }
    #endregion

    #region API Methods
    public void GetProfile(string userId)
    {
        var req = new ProfileRequest { userId = userId };
        SendWebGLMessage("profile", "getProfile", JsonUtility.ToJson(req));
    }

    public void SendSpawn(Vector3 position)
    {
        var payload = new {
            method = "spawn",
            @params = new { position = new { x = position.x, y = position.y, z = position.z } }
        };
        SendWebGLMessage("game", "mirrorAction", JsonUtility.ToJson(payload));
    }

    public void SendMove(List<Vector3> trajectory)
    {
        var wrapped = new List<Vector3Wrapper>();
        foreach (var v in trajectory)
            wrapped.Add(new Vector3Wrapper { x = v.x, y = v.y, z = v.z });

        var payload = new {
            method = "move",
            @params = new { trajectory = wrapped }
        };
        SendWebGLMessage("game", "mirrorAction", JsonUtility.ToJson(payload));
    }
    #endregion

    public bool IsInitialized() => IsSocketInitialized() == 1;

    void OnDestroy() => DestroySocket();
}
