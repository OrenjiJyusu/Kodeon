using UnityEngine;
using Mirror;
using kcp2k;

public class ServerStartup : MonoBehaviour
{

    [SerializeField] private BackendCommunicator backend;
    [SerializeField] private NetworkManager networkManager;
    private string myServerId;

       void Start()
    {
        // Only run logic if this is a dedicated server
        if (Application.isBatchMode || Application.platform == RuntimePlatform.LinuxServer || Application.platform == RuntimePlatform.WindowsServer) 
        {
            Debug.Log("Starting Dedicated Server...");
            
            // Get port from args or default
            int port = 7777; // Default Mirror port
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == "-port" && i + 1 < args.Length) {
                    int.TryParse(args[i + 1], out port);
                }
            }

            // Start Mirror Server
            if (Transport.active is KcpTransport kcp) {
                kcp.Port = (ushort)port;
            } else {
                Debug.LogWarning("Transport is not KcpTransport, using default port settings.");
            }
            networkManager.StartServer();
            
            // Force load the Online Scene (Game Map) if set
            if (!string.IsNullOrEmpty(networkManager.onlineScene))
            {
                Debug.Log($"[ServerStartup] Switching Server to Online Scene: '{networkManager.onlineScene}'");
                networkManager.ServerChangeScene(networkManager.onlineScene);
            }
            else
            {
                Debug.LogError("[ServerStartup] Online Scene is NOT set in NetworkManager! Server will stay in MainMenu.");
            }

            // Register with Backend
            string serverName = "Dedicated Server " + port;
            backend.RegisterServer(serverName, true, port, OnRegistered, OnError);
        }
    }

    void OnRegistered(GameServerData data) {
        Debug.Log($"Server Registered! ID: {data.serverId}");
        myServerId = data.serverId;
        InvokeRepeating(nameof(SendHeartbeat), 5f, 5f);
    }

    void OnError(string error) {
        Debug.LogError($"Failed to register server: {error}");
        // Retry logic could go here
    }

    void SendHeartbeat() {
        if (!string.IsNullOrEmpty(myServerId)) {
            backend.SendHeartbeat(myServerId);
        }
    }
}
