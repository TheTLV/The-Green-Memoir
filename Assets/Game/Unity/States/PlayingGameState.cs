using TheGreenMemoir.Core.Application.States;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.States
{
    /// <summary>
    /// Playing State - State Pattern
    /// Game đang chơi bình thường
    /// </summary>
    public class PlayingGameState : BaseGameState
    {
        public override string StateName => "Playing";

        private IEventBus _eventBus;

        public PlayingGameState(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Enter()
        {
            UnityEngine.Time.timeScale = 1f;
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        public override void Exit()
        {
            // Cleanup nếu cần
        }

        public override bool CanTransitionTo(IGameState newState)
        {
            // Playing có thể chuyển sang Menu (pause), Dialogue, hoặc Inventory
            return newState.StateName == "Menu" ||
                   newState.StateName == "Dialogue" ||
                   newState.StateName == "Inventory" ||
                   newState.StateName == "Shop";
        }
    }
}

