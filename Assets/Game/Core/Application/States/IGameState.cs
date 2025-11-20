namespace TheGreenMemoir.Core.Application.States
{
    /// <summary>
    /// Interface cho Game State - State Pattern
    /// Quản lý các trạng thái của game (Menu, Playing, Paused, Dialogue, etc.)
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Tên state (để debug)
        /// </summary>
        string StateName { get; }

        /// <summary>
        /// Vào state này
        /// </summary>
        void Enter();

        /// <summary>
        /// Thoát khỏi state này
        /// </summary>
        void Exit();

        /// <summary>
        /// Update state (gọi mỗi frame)
        /// </summary>
        void Update();

        /// <summary>
        /// Kiểm tra xem có thể chuyển sang state khác không
        /// </summary>
        bool CanTransitionTo(IGameState newState);
    }
}

