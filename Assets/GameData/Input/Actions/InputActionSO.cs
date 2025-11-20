using UnityEngine;

namespace TheGreenMemoir.Unity.Input
{
    [CreateAssetMenu(fileName = "InputAction", menuName = "Game/Input Action", order = 20)]
	public class InputActionSO : ScriptableObject
	{
		[Header("Info")]
		public string actionId = "action_id";
		public string displayName = "Action";
		public KeyCode key = KeyCode.None;
        public bool enabled = true;
        public InputActionType actionType = InputActionType.Custom;
        public InputActionGroup group = InputActionGroup.Custom;
        public enum InputKeyMode { Press, Hold, Toggle }
        [Header("Mode")]
        [Tooltip("Press: nhấn một lần\nHold: giữ để kích hoạt liên tục\nToggle: nhấn để bật/tắt trạng thái")]
        public InputKeyMode inputMode = InputKeyMode.Press;
        [Tooltip("Thời gian giữ tối thiểu (giây) để tính là Hold, chống spam")] public float holdDurationThreshold = 0f;
        [Tooltip("(Legacy) isContinuous duy trì để tương thích cũ. Nếu bật, mặc định coi như Hold.")]
        public bool isContinuous;
        [Tooltip("Cooldown (giây). Trong thời gian này, hành động sẽ không được kích lại.")]
        public float cooldownSeconds = 0f;

		[Header("Linked SO (optional)")]
		public ActionSOBase linkedAction;

        [Header("Animator (optional)")]
        public string animatorTriggerName;
	}
}


