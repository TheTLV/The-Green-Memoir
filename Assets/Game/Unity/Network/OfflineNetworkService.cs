using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Offline Network Service - Chạy game offline, không cần server
    /// </summary>
    public class OfflineNetworkService : MonoBehaviour, INetworkService
    {
        private bool _isInitialized = false;
        private string _playerId;
        private string _playerName = "Player";

        public bool IsConnected => _isInitialized;
        public bool IsHost => true; // Offline mode luôn là host
        public string PlayerId => _playerId;
        public string PlayerName => _playerName;
        public List<string> ConnectedPlayers => new List<string> { _playerId }; // Chỉ có mình

        public void Initialize(bool onlineMode)
        {
            if (onlineMode)
            {
                Debug.LogWarning("OfflineNetworkService initialized but onlineMode is true! Switching to offline mode.");
            }

            // Tạo player ID từ PlayerPrefs hoặc tạo mới
            _playerId = PlayerPrefs.GetString("PlayerId", System.Guid.NewGuid().ToString());
            PlayerPrefs.SetString("PlayerId", _playerId);

            _playerName = PlayerPrefs.GetString("PlayerName", "Player");

            _isInitialized = true;
            Debug.Log($"Offline mode initialized. Player ID: {_playerId}");
        }

        public void StartHost()
        {
            Debug.Log("Offline mode: Already hosting (single player)");
        }

        public void ConnectToServer(string address = null, int port = 0)
        {
            Debug.LogWarning("Cannot connect to server in offline mode!");
        }

        public void Disconnect()
        {
            Debug.Log("Offline mode: Disconnected (but still playing offline)");
        }

        public void SendMessageToServer(string message, object data = null)
        {
            // Offline mode: không gửi đi đâu, chỉ log
            Debug.Log($"Offline mode: Message '{message}' (not sent)");
        }

        public void SendMessageToClient(string clientId, string message, object data = null)
        {
            Debug.LogWarning("Cannot send message to client in offline mode!");
        }

        public void SyncGameState(object gameState)
        {
            // Offline mode: không sync, chỉ lưu local
            Debug.Log("Offline mode: Game state saved locally (not synced)");
        }

        public object GetGameState()
        {
            // Offline mode: trả về null hoặc local state
            return null;
        }
    }
}

