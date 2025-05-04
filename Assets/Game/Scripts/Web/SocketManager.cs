using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class SocketManager : MonoBehaviour
{
    // События для уведомления об успешном или неудачном получении профиля
    public event Action<ProfileResponse> OnProfileSuccess;
    public event Action<string> OnProfileError;

    [DllImport("__Internal")]
    private static extern void InitializeSocket(
        System.Action<string> getProfileCallback,
        System.Action<string> profileReceivedCallback
    );

    [DllImport("__Internal")]
    private static extern void CallSocketMethod(string methodName, string data);

    [DllImport("__Internal")]
    private static extern void CallSocketMethodWithCallback(string methodName, string data);

    [DllImport("__Internal")]
    private static extern void RequestProfile(string userId);

    [DllImport("__Internal")]
    private static extern void DestroySocket();

    void Start()
    {
        InitializeSocket(OnGetProfile, OnProfileReceived);
    }

    private void OnGetProfile(string jsonData)
    {
        Debug.Log($"SocketManager: Preparing to get profile with data: {jsonData}");
        // Тут можно выполнить дополнительную обработку перед отправкой запроса
    }

    private void OnProfileReceived(string jsonData)
    {
        Debug.Log($"SocketManager: Profile data received: {jsonData}");
        
        try {
            // Проверяем есть ли сообщение об ошибке
            ErrorResponse errorCheck = JsonUtility.FromJson<ErrorResponse>(jsonData);
            if (!string.IsNullOrEmpty(errorCheck.error)) {
                OnProfileError?.Invoke(errorCheck.error);
                return;
            }
            
            // Если нет ошибки, обрабатываем как обычный ответ
            ProfileResponse response = JsonUtility.FromJson<ProfileResponse>(jsonData);
            OnProfileSuccess?.Invoke(response);
        }
        catch (Exception e) {
            Debug.LogError($"Error parsing profile response: {e.Message}");
            OnProfileError?.Invoke("Failed to parse response");
        }
    }

    public void GetProfile(string userId)
    {
        RequestProfile(userId);
    }

    public void CallMethod(string methodName, string jsonData)
    {
        CallSocketMethod(methodName, jsonData);
    }

    public void CallMethodWithCallback(string methodName, string jsonData)
    {
        CallSocketMethodWithCallback(methodName, jsonData);
    }

    void OnDestroy()
    {
        DestroySocket();
    }

    [Serializable]
    public class ProfileRequest
    {
        public string userId;
    }

    [Serializable]
    public class ProfileResponse
    {
        public string id;
        public string username;
        public string email;
    }

    [Serializable]
    public class ErrorResponse
    {
        public string error;
    }
}
