using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Network Manager - Foundation cho multiplayer
    /// Hỗ trợ cả Unity Netcode và Mirror Networking
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance => _instance;

        [Header("Network Settings")]
        [SerializeField] private bool enableMultiplayer = false;
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
        [SerializeField] private int maxPlayers = 4;
#pragma warning restore CS0414
        [SerializeField] private string serverAddress = "localhost";
        [SerializeField] private int serverPort = 7777;

        [Header("Player Settings")]
        [SerializeField] private string playerName = "Player";
        [SerializeField] private string playerId;

        private bool _isConnected = false;
        private bool _isHost = false;
        private List<string> _connectedPlayers = new List<string>();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                GeneratePlayerId();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Tạo Player ID unique
        /// </summary>
        private void GeneratePlayerId()
        {
            if (string.IsNullOrEmpty(playerId))
            {
                playerId = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("PlayerId", playerId);
            }
            else
            {
                playerId = PlayerPrefs.GetString("PlayerId", System.Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Bật/tắt multiplayer
        /// </summary>
        public void SetMultiplayerEnabled(bool enabled)
        {
            enableMultiplayer = enabled;
        }

        /// <summary>
        /// Tạo server (Host)
        /// </summary>
        public void StartHost()
        {
            if (!enableMultiplayer)
            {
                Debug.LogWarning("Multiplayer is disabled!");
                return;
            }

            _isHost = true;
            _isConnected = true;
            _connectedPlayers.Clear();
            _connectedPlayers.Add(playerId);

            Debug.Log($"Server started on {serverAddress}:{serverPort}");
            OnServerStarted();
        }

        /// <summary>
        /// Kết nối đến server
        /// </summary>
        public void ConnectToServer(string address = null, int port = 0)
        {
            if (!enableMultiplayer)
            {
                Debug.LogWarning("Multiplayer is disabled!");
                return;
            }

            if (!string.IsNullOrEmpty(address))
                serverAddress = address;
            if (port > 0)
                serverPort = port;

            _isConnected = true;
            Debug.Log($"Connecting to {serverAddress}:{serverPort}");
            OnClientConnected();
        }

        /// <summary>
        /// Ngắt kết nối
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
            _isHost = false;
            _connectedPlayers.Clear();
            Debug.Log("Disconnected from server");
            OnClientDisconnected();
        }

        /// <summary>
        /// Gửi message đến server
        /// </summary>
        public void SendMessageToServer(string message, object data = null)
        {
            if (!_isConnected) return;

            // TODO: Implement actual network message sending
            Debug.Log($"Sending message to server: {message}");
        }

        /// <summary>
        /// Gửi message đến client
        /// </summary>
        public void SendMessageToClient(string clientId, string message, object data = null)
        {
            if (!_isHost) return;

            // TODO: Implement actual network message sending
            Debug.Log($"Sending message to client {clientId}: {message}");
        }

        // Events
        private void OnServerStarted()
        {
            // TODO: Implement server started logic
        }

        private void OnClientConnected()
        {
            // TODO: Implement client connected logic
        }

        private void OnClientDisconnected()
        {
            // TODO: Implement client disconnected logic
        }

        // Properties
        public bool IsConnected => _isConnected;
        public bool IsHost => _isHost;
        public string PlayerId => playerId;
        public string PlayerName => playerName;
        public List<string> ConnectedPlayers => _connectedPlayers;
    }
}

