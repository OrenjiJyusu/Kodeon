using UnityEngine;
using UnityEngine.UI;
using Mirror;
using kcp2k;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BackendCommunicator backend;
    [SerializeField] private NetworkManager networkManager;
    
    [Header("UI Elements")]
    [SerializeField] private Button findMatchButton;
    [SerializeField] private TMP_Text statusText;

    void Start()
    {
        // Auto-find references if missing (Robustness for persistent singleton)
        if (networkManager == null) {
            networkManager = NetworkManager.singleton;
        }
        
        if (backend == null && networkManager != null) {
            backend = networkManager.GetComponent<BackendCommunicator>();
        }

        if (findMatchButton != null) {
            findMatchButton.onClick.AddListener(OnFindMatchClicked);
        }
    }

    void OnFindMatchClicked()
    {
        statusText.text = "Searching for game...";
        findMatchButton.interactable = false;

        backend.FindServer(OnServerFound, OnError);
    }

    void OnServerFound(GameServerData data)
    {
        if (data == null)
        {
            statusText.text = "No servers online. Try starting a headless server locally.";
            findMatchButton.interactable = true;
            return;
        }

        statusText.text = $"Connecting to {data.ipAddress}:{data.port}...";
        Debug.Log($"Connecting to {data.ipAddress}:{data.port}");

        // Configure Mirror Connection
        networkManager.networkAddress = data.ipAddress;

        if (Transport.active is KcpTransport kcp) {
            kcp.Port = (ushort)data.port;
        }
        
        networkManager.StartClient();
    }

    void OnError(string error)
    {
        statusText.text = $"Connection Error: {error}";
        Debug.LogError(error);
        findMatchButton.interactable = true;
    }
}
