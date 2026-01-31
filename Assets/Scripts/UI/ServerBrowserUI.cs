using UnityEngine;
using UnityEngine.UI;
using Mirror;
using kcp2k;
using TMPro;
using System.Collections.Generic;

public class ServerBrowserUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BackendCommunicator backend;
    [SerializeField] private NetworkManager networkManager;
    
    [Header("UI Elements")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton; // To join by IP manually if needed
    [SerializeField] private TMP_InputField hostNameInput;
    [SerializeField] private Transform serverListContainer;
    [SerializeField] private GameObject serverEntryPrefab; // A prefab with button + text
    [SerializeField] private TMP_Text statusText;

    private List<GameServerData> currentServers = new List<GameServerData>();

    void Start()
    {
        // Auto-find references
        if (networkManager == null) networkManager = NetworkManager.singleton;
        if (backend == null && networkManager != null) backend = networkManager.GetComponent<BackendCommunicator>();

        refreshButton.onClick.AddListener(RefreshServerList);
        hostButton.onClick.AddListener(HostGame);
    }

    void RefreshServerList()
    {
        statusText.text = "Fetching servers...";
        refreshButton.interactable = false;

        // Clear current list UI
        foreach (Transform child in serverListContainer) {
            Destroy(child.gameObject);
        }

        backend.GetServerList(OnServerListReceived, OnError);
    }

    void OnServerListReceived(List<GameServerData> servers)
    {
        refreshButton.interactable = true;
        statusText.text = $"Found {servers.Count} servers.";
        currentServers = servers;

        foreach (var server in servers)
        {
            GameObject entry = Instantiate(serverEntryPrefab, serverListContainer);
            TMP_Text label = entry.GetComponentInChildren<TMP_Text>();
            Button btn = entry.GetComponentInChildren<Button>();
            
            label.text = $"{server.name} | Players: {server.playerCount}/{server.maxPlayers}";
            btn.onClick.AddListener(() => JoinServer(server));
        }
    }

    void JoinServer(GameServerData server)
    {
        statusText.text = $"Joining {server.name}...";
        networkManager.networkAddress = server.ipAddress;
        
        if (Transport.active is KcpTransport kcp) {
            kcp.Port = (ushort)server.port;
        }

        networkManager.StartClient();
    }

    void HostGame()
    {
        string name = hostNameInput.text;
        if (string.IsNullOrEmpty(name)) name = "My Game";

        statusText.text = "Starting Host...";
        
        // Start Mirror Host
        // Use a random port to avoid conflict with Dedicated Server (7777) logic
        int port = UnityEngine.Random.Range(8000, 9999);
        
        if (Transport.active is KcpTransport kcp) {
            kcp.Port = (ushort)port;
        }

        networkManager.StartHost();

        backend.RegisterServer(name, false, port, (data) => {
             Debug.Log("Host Registered with Backend!");
             // Save ID to unregister later
             PlayerPrefs.SetString("MyServerId", data.serverId);
             
             // Start Heartbeat for Host
             InvokeRepeating(nameof(SendHeartbeat), 5f, 5f);
        }, OnError);
    }

    void SendHeartbeat() {
        string id = PlayerPrefs.GetString("MyServerId", "");
        if (!string.IsNullOrEmpty(id)) {
            backend.SendHeartbeat(id);
        }
    }

    void OnError(string error)
    {
        statusText.text = $"Error: {error}";
        refreshButton.interactable = true;
    }
}
