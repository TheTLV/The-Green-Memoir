using UnityEngine;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.Interaction
{
	/// <summary>
	/// Quy tắc chuyển trạng thái: (fromState + action) -> toState
	/// Có thể gán điều kiện đơn giản và VFX/SFX sau này nếu cần.
	/// </summary>
	[CreateAssetMenu(fileName = "InteractionTransition", menuName = "Game/Interaction/Transition", order = 42)]
	public class InteractionTransitionSO : ScriptableObject
	{
		public InteractionStateSO fromState;
		public InteractionActionSO action;
		public InteractionStateSO toState;

		[Header("Optional")]
		public float minCooldown;
		public string animatorTriggerName;

		[Header("Audio (optional)")]
		[Tooltip("Âm thanh phát khi transition này xảy ra")]
		public AudioClipSO audioClip;
	}
}


