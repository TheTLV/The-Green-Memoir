using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Online Network Service - Chạy game online, cần server
    /// Có thể tích hợp Unity Netcode, Mirror, hoặc custom network solution
    /// </summary>
    public class OnlineNetworkService : MonoBehaviour, INetworkService
    {
        [Header("Network Settings")]
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
        [SerializeField] private int maxPlayers = 4;
#pragma warning restore CS0414
        [SerializeField] private string serverAddress = "localhost";
        [SerializeField] private int serverPort = 7777;

        private bool _isConnected = false;
        private bool _isHost = false;
        private string _playerId;
        private string _playerName = "Player";
        private List<string> _connectedPlayers = new List<string>();

        public bool IsConnected => _isConnected;
        public bool IsHost => _isHost;
        public string PlayerId => _playerId;
        public string PlayerName => _playerName;
        public List<string> ConnectedPlayers => _connectedPlayers;

        public void Initialize(bool onlineMode)
        {
            if (!onlineMode)
            {
                Debug.LogWarning("OnlineNetworkService initialized but onlineMode is false! Use OfflineNetworkService instead.");
                return;
            }

            _playerId = PlayerPrefs.GetString("PlayerId", System.Guid.NewGuid().ToString());
            PlayerPrefs.SetString("PlayerId", _playerId);

            _playerName = PlayerPrefs.GetString("PlayerName", "Player");

            Debug.Log($"Online mode initialized. Player ID: {_playerId}");
            // TODO: Initialize actual network library (Unity Netcode, Mirror, etc.)
        }

        public void StartHost()
        {
            _isHost = true;
            _isConnected = true;
            _connectedPlayers.Clear();
            _connectedPlayers.Add(_playerId);

            Debug.Log($"Server started on {serverAddress}:{serverPort}");
            // TODO: Start actual server
        }

        public void ConnectToServer(string address = null, int port = 0)
        {
            if (!string.IsNullOrEmpty(address))
                serverAddress = address;
            if (port > 0)
                serverPort = port;

            _isConnected = true;
            Debug.Log($"Connecting to {serverAddress}:{serverPort}");
            // TODO: Connect to actual server
        }

        public void Disconnect()
        {
            _isConnected = false;
            _isHost = false;
            _connectedPlayers.Clear();
            Debug.Log("Disconnected from server");
            // TODO: Disconnect from actual server
        }

        public void SendMessageToServer(string message, object data = null)
        {
            if (!_isConnected) return;

            Debug.Log($"Sending message to server: {message}");
            // TODO: Send actual network message
        }

        public void SendMessageToClient(string clientId, string message, object data = null)
        {
            if (!_isHost) return;

            Debug.Log($"Sending message to client {clientId}: {message}");
            // TODO: Send actual network message
        }

        public void SyncGameState(object gameState)
        {
            if (!_isConnected) return;

            Debug.Log("Syncing game state to server");
            // TODO: Sync actual game state
        }

        public object GetGameState()
        {
            if (!_isConnected) return null;

            Debug.Log("Getting game state from server");
            // TODO: Get actual game state from server
            return null;
        }
    }
}

