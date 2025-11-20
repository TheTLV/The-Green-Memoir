using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Interface cho Network Service - Abstraction layer
    /// Cho phép chuyển đổi giữa offline và online mode dễ dàng
    /// </summary>
    public interface INetworkService
    {
        bool IsConnected { get; }
        bool IsHost { get; }
        string PlayerId { get; }
        string PlayerName { get; }
        List<string> ConnectedPlayers { get; }

        /// <summary>
        /// Khởi tạo service (offline hoặc online)
        /// </summary>
        void Initialize(bool onlineMode);

        /// <summary>
        /// Tạo server (chỉ online mode)
        /// </summary>
        void StartHost();

        /// <summary>
        /// Kết nối đến server (chỉ online mode)
        /// </summary>
        void ConnectToServer(string address = null, int port = 0);

        /// <summary>
        /// Ngắt kết nối
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gửi message đến server
        /// </summary>
        void SendMessageToServer(string message, object data = null);

        /// <summary>
        /// Gửi message đến client
        /// </summary>
        void SendMessageToClient(string clientId, string message, object data = null);

        /// <summary>
        /// Sync game state (online mode)
        /// </summary>
        void SyncGameState(object gameState);

        /// <summary>
        /// Lấy game state từ server (online mode)
        /// </summary>
        object GetGameState();
    }
}

