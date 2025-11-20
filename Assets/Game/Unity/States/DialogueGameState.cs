using TheGreenMemoir.Core.Application.States;

namespace TheGreenMemoir.Unity.States
{
    /// <summary>
    /// Dialogue State - State Pattern
    /// Game đang trong dialogue/conversation
    /// </summary>
    public class DialogueGameState : BaseGameState
    {
        public override string StateName => "Dialogue";

        public override void Enter()
        {
            UnityEngine.Time.timeScale = 0f; // Pause game khi dialogue
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }

        public override void Exit()
        {
            UnityEngine.Time.timeScale = 1f;
        }

        public override bool CanTransitionTo(IGameState newState)
        {
            // Dialogue chỉ có thể quay về Playing
            return newState.StateName == "Playing";
        }
    }
}

