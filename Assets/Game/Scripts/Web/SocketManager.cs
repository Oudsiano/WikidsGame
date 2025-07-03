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
    [DllImport("__Internal")] private static extern void GetUserProfile();
    
    [DllImport("__Internal")] private static extern void FastJoinToRoom();
    
    [DllImport("__Internal")] private static extern void ConfirmRoom();
    
    [DllImport("__Internal")] private static extern void SendHealth(string id, float health);
    
    [DllImport("__Internal")] private static extern void GetAllPlayersInScene(string id);
    
    [DllImport("__Internal")] private static extern string CallNamespacedMethod(string ns, string method);
    
    [DllImport("__Internal")] public static extern string CallNamespacedMethodWithArg(string ns, string methodName, string jsonArg);
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
    [Serializable] public class MirrorMoveAction { public string method;
        public string playerId; public MoveParams @params; }

    [Serializable] public class SpawnParams { public Vector3Wrapper position; }
    
    [Serializable] public class HealthParams{ public float health; }
    
    [Serializable] public class SpawnActionWrapper { public string method; public string playerId;  public SpawnParams @params; }
    
    public static Dictionary<string, Vector3> OtherPlayersPositions = new();
    
    public static string MyLocalPlayerId;
    public static GameObject LocalPlayerObject;
    
    [Serializable]
    public class PlayerInfo
    {
        public string _id;
        public string username;
        public string avatarUrl;
    }
    
    [System.Serializable]
    public class EventData
    {
        public string @event;
    }

    [Serializable]
    public class RoomStateWrapper
    {
        public string status;
        public PlayerInfo[] players;
    }
    
    [Serializable]
    public class PositionParams
    {
        public Vector3Wrapper position;
    }

    [Serializable]
    public class MirrorPositionAction
    {
        public string method;
        public string playerId;
        public PositionParams @params;
    }
    
    [Serializable]
    public class MirrorHealthAction
    {
        public string method;
        public string playerId;
        public HealthParams @params;
    }
    
    [Serializable]
    private class GenericMirrorAction
    {
        public string method;
        public string playerId;
    }
    #endregion

    
    public void Construct()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterCallbacks();
        InitializeSocketWithCallbacks();
        TestJSLib();
        // FastJoinToRoom();
        // CallNamespacedMethod("room", "fastJoin");
        
#endif
    }

    public static void GetUserProfileInGame()
    {
        Debug.Log("[SocketManager] GetUserProfile called");
        GetUserProfile();
    }
    
    public static void RequestAllPlayersInScene()
    {
        Debug.Log("[SocketManager] Requesting all players in scene with my id"+ MyLocalPlayerId);

#if UNITY_WEBGL && !UNITY_EDITOR
    GetAllPlayersInScene(MyLocalPlayerId);
#else
        Debug.Log($"[RequestAllPlayersInScene] Would call GetAllPlayersInScene with playerId: {MyLocalPlayerId}");
#endif
    }

    public static void SendUserHealthInGame(string playerId, float health)
    {
        Debug.Log("[SocketManager] SendUserHealthInGame called");
        Debug.Log("Send health: " + health);
        SendHealth(playerId, health);
    }
    
    public static void SendPlayerTrajectory(List<Vector3> trajectory)
    {
        if (string.IsNullOrEmpty(MyLocalPlayerId))
        {
            Debug.LogWarning("[SocketManager] Cannot send trajectory — playerId is null or empty.");
            return;
        }

        List<Vector3Wrapper> wrappedTrajectory = new List<Vector3Wrapper>();
        foreach (var point in trajectory)
        {
            wrappedTrajectory.Add(new Vector3Wrapper
            {
                x = point.x,
                y = point.y,
                z = point.z
            });
        }

        MirrorMoveAction moveAction = new MirrorMoveAction
        {
            method = "move",
            playerId = MyLocalPlayerId,
            @params = new MoveParams
            {
                trajectory = wrappedTrajectory
            }
        };

        string json = JsonUtility.ToJson(moveAction);
        Debug.Log($"[SocketManager] Sending full trajectory: {json}");

#if UNITY_WEBGL && !UNITY_EDITOR
    CallNamespacedMethodWithArg("game", "mirrorAction", json);
#else
        Debug.Log($"[SendPlayerTrajectory] Would send: {json}");
#endif
    }
    
    public static void SendPlayerHealth(float health)
    {
        if (string.IsNullOrEmpty(MyLocalPlayerId))
        {
            Debug.LogWarning("[SocketManager] Cannot send health — playerId is null or empty.");
            return;
        }

        MirrorHealthAction healthAction = new MirrorHealthAction
        {
            method = "sendHealth",
            playerId = MyLocalPlayerId,
            @params = new HealthParams
            {
                health = health
            }
        };

        string json = JsonUtility.ToJson(healthAction);
        Debug.Log($"[SocketManager] Sending health: {json}");

#if UNITY_WEBGL && !UNITY_EDITOR
    CallNamespacedMethodWithArg("game", "mirrorAction", json);
#else
        Debug.Log($"[SendPlayerHealth] Would send: {json}");
#endif
    }
    
    public static void SendPlayerPosition(Vector3 position)
    {
        if (string.IsNullOrEmpty(MyLocalPlayerId))
        {
            Debug.LogWarning("[SocketManager] Cannot send position — playerId is null or empty.");
            return;
        }
        
        Debug.Log("[SocketManager] SendPlayerPosition");
        string jsonArg = $@"
        {{
            ""method"": ""sendPosition"",
            ""playerId"": ""{MyLocalPlayerId}"",
            ""params"": {{
                ""position"": {{
                    ""x"": {position.x.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)},
                    ""y"": {position.y.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)},
                    ""z"": {position.z.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)}
                }}
            }}
        }}";

#if UNITY_WEBGL && !UNITY_EDITOR
    CallNamespacedMethodWithArg("game", "mirrorAction", jsonArg);
#else
        Debug.Log($"[SendOwnPosition] Would send: {jsonArg}");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void RegisterCallbacks()
    {
        // Profile
        RegisterUnityCallback("user", "onProfileReceived", OnProfileReceivedCallback);
        
        // Room
         RegisterUnityCallback("room", "onFastJoinRoomReceived", OnFastJoinRoomReceivedCallback);
        RegisterUnityCallback("room", "onLeaveRoomReceived", OnLeaveRoomReceivedCallback);
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
    
    public static void TrySetPlayerIdManually()
    {
        Debug.Log("[SocketManager] Attempting to set playerId manually...");
        
        string json = CallNamespacedMethod("user", "getProfile");
        if (json.Contains("error"))
        {
            Debug.LogWarning("[SocketManager] Failed to get profile manually.");
            return;
        }

        try
        {
            var profile = JsonUtility.FromJson<ProfileResponse>(json);
            MyLocalPlayerId = profile.id;
            Debug.Log($"[SocketManager] [Manual Fallback] My playerId set to: {MyLocalPlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError("[SocketManager] Manual profile parse error: " + e.Message);
        }
    }
    
    #region Callback Implementations
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnProfileReceivedCallback(string json)
    {
        Debug.Log("[SocketManager] OnProfileReceivedCallback: " + json);;
        if (json.Contains("error"))
        {
            OnError?.Invoke(json);
            return;
        }

        try
        {
            var profile = JsonUtility.FromJson<ProfileResponse>(json);
            MyLocalPlayerId = profile.id;
            Debug.Log($"[SocketManager] My playerId set to: {MyLocalPlayerId}");
            FastJoinToRoom();

            OnProfileReceived?.Invoke(json); // Оставляем вызов события
        }
        catch (Exception e)
        {
            Debug.LogError($"[SocketManager] Failed to parse ProfileResponse: {e.Message}");
            OnError?.Invoke(json);
        }
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnGameStateChangedCallback(string json)
    {
        if (json.Contains("error")) { OnError?.Invoke(json); return; }
        
        if (json.Contains("event"))
        {
            var eventData = JsonUtility.FromJson<EventData>(json); // Создай класс EventData с полем event
            if (eventData.@event == "onPlayingPhaseStart")
            {
                Debug.Log("[SOCKETMANAGER] Handling onPlayingPhaseStart");
                // Обработай событие
            }
            else if (eventData.@event == "onPlayerAction")
            {
                var baseAction = JsonUtility.FromJson<MirrorBaseAction>(json);
                ProcessMirrorAction(baseAction, json);
            }
            else if (eventData.@event == "onDropPlayer")
            {
                Debug.Log("[SOCKETMANAGER] Handling onDropPlayer");
                MultiplayerController.Instance.DestroyOtherPlayer();

            }
            
            OnGameStateChanged?.Invoke(json);
            return;
        }
        
        var BaseAction = JsonUtility.FromJson<MirrorBaseAction>(json);
        ProcessMirrorAction(BaseAction, json);
        OnGameStateChanged?.Invoke(json);
    }
    
    private static void ProcessMirrorAction(MirrorBaseAction baseAction, string json)
    {
        switch (baseAction.method)
        {
            case "move": HandleMove(json); break;
            case "spawn": HandleSpawn(json); break;
            case "sendPosition":
                Debug.Log("[SOCKETMANAGER] OnGameStateChangedCallback - sendPosition");
                var posAction = JsonUtility.FromJson<MirrorPositionAction>(json);
            
                if (posAction.playerId == MyLocalPlayerId)
                {
                    Debug.Log("[SOCKETMANAGER] Ignoring own position update.");
                    break;
                }
                
                var pos = posAction.@params.position;
                var position = new Vector3(pos.x, pos.y, pos.z);
                
                OtherPlayersPositions[posAction.playerId] = position;
                
                MultiplayerController.Instance?.MoveOtherPlayerAlongPath(position);
                Debug.Log($"[SOCKETMANAGER] Got player position: {position}");
                break;
            case "sendHealth":
                HandleHealth(json);
                break;
            case "getAllPlayersInScene":
                HandleGetAllPlayersInScene(json);
                break;
            default:
                Debug.LogWarning($"[OnGameStateChangedCallback] Unknown mirrorAction: {baseAction.method}, JSON: {json}");
                break;
        }
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
        
        if (string.IsNullOrEmpty(json) || json == "{}")
        {
            Debug.LogWarning("Empty JSON response received for mirrorAction");
            OnMirrorActionResponse?.Invoke(json);
            return;
        }
        

        var baseAction = JsonUtility.FromJson<MirrorBaseAction>(json);

        switch (baseAction.method)
        {
            case "move": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - MOVE "); break;
            case "spawn": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - spawn "); break;
            case "sendPosition": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - sendPosition "); break;
            case "sendHealth": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - sendHealth "); break;
            case "getAllPlayersInScene": Debug.Log("[SOCKETMANAGER] OnMirrorActionResponseCallback - getAllPlayersInScene "); break;
            default:
                Debug.LogWarning($"Unknown mirrorAction (response): {baseAction.method}, JSON: {json}");
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
    private static void OnLeaveRoomReceivedCallback(string json)
    {
        Debug.Log("[SOCKETMANAGER] Player left room: " + json);
        OnLeaveRoomReceived?.Invoke(json);

        // Здесь можно обработать логику выхода соперника (например, возврат в главное меню или уведомление)
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
                // CallNamespacedMethod("room", "confirm");
                ConfirmRoom();

                foreach (var playerInfo in state.players)
                {
                    if (playerInfo._id != MyLocalPlayerId)
                    {
                        Debug.Log($"[SOCKETMANAGER] Found other player in room: {playerInfo._id}");
                        
                        Vector3 spawnPos = OtherPlayersPositions.TryGetValue(playerInfo._id, out var savedPos)
                            ? savedPos
                            : new Vector3(195, -24.00106f, 38.42f);
                
                        // Заспавним другого игрока (можешь потом сделать позицию умной — сейчас для теста фиксированную):
                        MultiplayerController.Instance?.SpawnOtherPlayer(playerInfo._id, spawnPos);
                    }
                }
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
        
        Debug.Log($"[SOCKETMANAGER] Received move: playerId = {action.playerId}, trajectoryCount = {action.@params?.trajectory?.Count ?? 0}");
        
        if (action.playerId == MyLocalPlayerId)
        {
            Debug.Log("[SOCKETMANAGER] Ignoring own trajectory update.");
            return;
        }
        
        if (action.@params?.trajectory == null)
        {
            Debug.LogWarning("Empty trajectory");
            return;
        }

        var path = new List<Vector3>();
        foreach (var point in action.@params.trajectory)
            path.Add(new Vector3(point.x, point.y, point.z));

        // MultiplayerController.Instance?.MoveOtherPlayerAlongPath(path);
    }

    private static void HandleSpawn(string json)
    {
        try
        {
            // CallNamespacedMethodWithArg("game", "mirrorAction", json);
            var spawn = JsonUtility.FromJson<SpawnActionWrapper>(json);
            var pos = spawn.@params.position;
            var otherPlayerId = spawn.playerId;

            if (otherPlayerId != MyLocalPlayerId)
            {
                Debug.Log("MyLocalPlayerId: " + MyLocalPlayerId + ", OtherPlayerId: " + otherPlayerId);
                MultiplayerController.Instance.SpawnOtherPlayer(otherPlayerId, new Vector3(pos.x, pos.y, pos.z));
                Debug.Log($"[SOCKETMANAGER] Spawned player at: {pos.x}, {pos.y}, {pos.z}");
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Spawn parse error: " + e.Message);
        }
    }
    
    private static void HandleHealth(string json)
    {
        var action = JsonUtility.FromJson<MirrorHealthAction>(json);

        Debug.Log($"[SOCKETMANAGER] Received health: playerId = {action.playerId}, health = {action.@params.health}");

        if (action.playerId == MyLocalPlayerId)
        {
            Debug.Log("[SOCKETMANAGER] Ignoring own health update.");
            return;
        }

        MultiplayerController.Instance?.UpdateOtherPlayerHealth(action.@params.health);
    }
    
    private static void HandleGetAllPlayersInScene(string json)
    {
        var action = JsonUtility.FromJson<GenericMirrorAction>(json);

        Debug.Log($"[SOCKETMANAGER] Received getAllPlayersInScene request from playerId = {action.playerId}");

        if (action.playerId == MyLocalPlayerId)
        {
            Debug.Log("[SOCKETMANAGER] Ignoring own getAllPlayersInScene request.");
            return;
        }

        if (LocalPlayerObject == null)
        {
            Debug.LogWarning("[SOCKETMANAGER] LocalPlayerObject is not set — can't respond with spawn.");
            return;
        }

        Vector3 myPosition = LocalPlayerObject.transform.position;

        Debug.Log($"[SOCKETMANAGER] Sending spawn response with my position: {myPosition}");

        string spawnJson = $@"
    {{
        ""method"": ""spawn"",
        ""playerId"": ""{MyLocalPlayerId}"",
        ""params"": {{
            ""position"": {{
                ""x"": {myPosition.x.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)},
                ""y"": {myPosition.y.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)},
                ""z"": {myPosition.z.ToString("F5", System.Globalization.CultureInfo.InvariantCulture)}
            }}
        }}
    }}";

#if UNITY_WEBGL && !UNITY_EDITOR
    CallNamespacedMethodWithArg("game", "mirrorAction", spawnJson);
#else
        Debug.Log($"[SOCKETMANAGER] HandleGetAllPlayersInScene ");
        Debug.Log($"[SOCKETMANAGER] Would send spawn: {spawnJson}");
#endif
    }

    
    #endregion

    #region API Methods



    #endregion

    public bool IsInitialized() => IsSocketInitialized() == 1;

    void OnDestroy() => DestroySocket();
}
