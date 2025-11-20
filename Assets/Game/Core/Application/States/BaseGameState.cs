namespace TheGreenMemoir.Core.Application.States
{
    /// <summary>
    /// Base class cho Game States - State Pattern
    /// Cung cấp implementation mặc định
    /// </summary>
    public abstract class BaseGameState : IGameState
    {
        public abstract string StateName { get; }

        public virtual void Enter()
        {
            // Override nếu cần
        }

        public virtual void Exit()
        {
            // Override nếu cần
        }

        public virtual void Update()
        {
            // Override nếu cần
        }

        public virtual bool CanTransitionTo(IGameState newState)
        {
            // Mặc định cho phép chuyển sang bất kỳ state nào
            // Override để thêm logic kiểm tra
            return true;
        }
    }
}

