using TheGreenMemoir.Core.Application.States;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.States
{
    /// <summary>
    /// Menu State - State Pattern
    /// Game đang ở menu (title screen, pause menu, etc.)
    /// </summary>
    public class MenuGameState : BaseGameState
    {
        public override string StateName => "Menu";

        private IEventBus _eventBus;

        public MenuGameState(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Enter()
        {
            UnityEngine.Time.timeScale = 0f; // Pause game
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }

        public override void Exit()
        {
            UnityEngine.Time.timeScale = 1f; // Resume game
        }

        public override bool CanTransitionTo(IGameState newState)
        {
            // Menu có thể chuyển sang Playing, Settings, hoặc Quit
            return newState.StateName == "Playing" || 
                   newState.StateName == "Settings" ||
                   newState.StateName == "Quit";
        }
    }
}

