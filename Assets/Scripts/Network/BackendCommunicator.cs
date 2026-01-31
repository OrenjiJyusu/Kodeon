using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class GameServerData {
    public string serverId;
    public string name;
    public bool isDedicated;
    public string ipAddress;
    public int port;
    public int playerCount;
    public int maxPlayers;
    public string status;
}

public class BackendCommunicator : MonoBehaviour
{
    private const string BACKEND_URL = "http://localhost:8080";

    public void RegisterServer(string serverName, bool isDedicated, int port, Action<GameServerData> onSuccess, Action<string> onError) {
        GameServerData data = new GameServerData {
            name = serverName,
            isDedicated = isDedicated,
            ipAddress = "127.0.0.1", 
            port = port,
            maxPlayers = 8,
            playerCount = 0
        };
        
        string json = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest("/server/register", json, onSuccess, onError));
    }

    public void GetServerList(Action<List<GameServerData>> onSuccess, Action<string> onError) {
        StartCoroutine(GetRequestList("/server/list", onSuccess, onError));
    }

    public void UnregisterServer(string serverId, Action onSuccess, Action<string> onError) {
        StartCoroutine(PostRequestNoResponse($"/server/unregister/{serverId}", "", onSuccess, onError));
    }

    public void SendHeartbeat(string serverId) {
        StartCoroutine(PostRequestNoResponse($"/server/heartbeat/{serverId}", ""));
    }

    public void FindServer(Action<GameServerData> onSuccess, Action<string> onError) {
        StartCoroutine(GetRequest("/server/find", onSuccess, onError));
    }

    private IEnumerator PostRequest(string endpoint, string json, Action<GameServerData> onSuccess, Action<string> onError) {
        string url = BACKEND_URL + endpoint;
        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                onError?.Invoke(request.error);
            } else {
                GameServerData result = JsonUtility.FromJson<GameServerData>(request.downloadHandler.text);
                onSuccess?.Invoke(result);
            }
        }
    }

    // Helper wrapper for List deserialization
    [Serializable]
    private class Wrapper<T> { public List<T> items; }

    private IEnumerator PostRequestNoResponse(string endpoint, string json, Action onSuccess = null, Action<string> onError = null) {
        string url = BACKEND_URL + endpoint;
        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) {
             if (!string.IsNullOrEmpty(json)) {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                onError?.Invoke(request.error);
            } else {
                onSuccess?.Invoke();
            }
        }
    }

    private IEnumerator GetRequestList(string endpoint, Action<List<GameServerData>> onSuccess, Action<string> onError) {
        string url = BACKEND_URL + endpoint;
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                onError?.Invoke(request.error);
            } else {
                // Unity default JsonUtility doesn't support top-level lists, need to wrap or use NewtonSoft
                // Hacky manual parsing for list or use Wrapper if backend returns { "items": [] }
                // Since our backend returns raw [], we need a simple workaround or switch to Newtonsoft.
                // For simplicity, let's assume valid JSON array handling via a wrapper isn't possible directly on [] without parsing.
                // We will use a simple regex/helper or just JsonConvert if available.
                // Fallback: Using a simple helper to wrap the array string into a field so JsonUtility can read it:
                string json = "{\"items\":" + request.downloadHandler.text + "}";
                Wrapper<GameServerData> wrapper = JsonUtility.FromJson<Wrapper<GameServerData>>(json);
                onSuccess?.Invoke(wrapper.items);
            }
        }
    }

    private IEnumerator GetRequest(string endpoint, Action<GameServerData> onSuccess, Action<string> onError) {
        string url = BACKEND_URL + endpoint;
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                onError?.Invoke(request.error);
            } else {
                // Check for 204 No Content
                if (request.responseCode == 204) {
                    onSuccess?.Invoke(null);
                } else {
                    GameServerData result = JsonUtility.FromJson<GameServerData>(request.downloadHandler.text);
                    onSuccess?.Invoke(result);
                }
            }
        }
    }
}
