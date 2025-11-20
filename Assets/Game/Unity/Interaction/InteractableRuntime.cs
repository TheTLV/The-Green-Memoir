using UnityEngine;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.Interaction
{
	/// <summary>
	/// Runtime component gắn trên object (tile proxy, cây, đá, ...)
	/// Lưu currentState và áp dụng transition theo InteractionGraphSO (rule-based).
	/// Kiểm tra điều kiện bị chặn trước khi tương tác.
	/// </summary>
	public class InteractableRuntime : MonoBehaviour
	{
		[Header("Graph & State")]
		[SerializeField] private InteractionGraphSO graph;
		[SerializeField] private InteractionStateSO currentState;
		[SerializeField] private Animator animatorRef;

		[Header("Blocking Conditions")]
		[Tooltip("Không cho tương tác khi đang pause")]
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
		[SerializeField] private bool blockOnPause = true;
#pragma warning restore CS0414
		[Tooltip("Không cho tương tác khi animator đang chạy animation")]
		[SerializeField] private bool blockOnAnimation = true;
		[Tooltip("Không cho tương tác khi GameState không cho phép input")]
		[SerializeField] private bool blockOnInputDisabled = true;

		[Header("Cooldown")]
		[Tooltip("Cooldown tối thiểu giữa các lần tương tác (giây)")]
		[SerializeField] private float interactionCooldown = 0f;

		private float _lastInteractionTime = 0f;

		public InteractionStateSO CurrentState => currentState;

		public void Initialize(InteractionGraphSO g, InteractionStateSO start)
		{
			graph = g; 
			currentState = start;
		}

		/// <summary>
		/// Thử áp dụng action lên object theo graph (rule-based transition).
		/// Kiểm tra điều kiện bị chặn trước khi thực hiện.
		/// Trả true nếu chuyển được.
		/// </summary>
		public bool TryApply(InteractionActionSO action)
		{
			// Kiểm tra điều kiện bị chặn
			if (!CanInteract()) return false;

			if (graph == null || currentState == null || action == null) return false;
			var t = graph.FindTransition(currentState, action);
			if (t == null || t.toState == null) return false;

			// Kiểm tra cooldown của transition
			if (t.minCooldown > 0f && Time.time - _lastInteractionTime < t.minCooldown)
			{
				return false;
			}

			// Animator trigger an toàn
			if (animatorRef != null && !string.IsNullOrWhiteSpace(t.animatorTriggerName))
			{
				// Kiểm tra animator không đang trong transition
				if (blockOnAnimation && animatorRef.IsInTransition(0))
				{
					return false;
				}

				animatorRef.ResetTrigger(t.animatorTriggerName);
				animatorRef.SetTrigger(t.animatorTriggerName);
			}

			// Phát âm thanh nếu có
			if (t.audioClip != null)
			{
				t.audioClip.Play(transform.position);
			}

			currentState = t.toState;
			_lastInteractionTime = Time.time;
			return true;
		}

		/// <summary>
		/// Kiểm tra xem có thể tương tác không (không bị chặn)
		/// </summary>
		private bool CanInteract()
		{
			// Kiểm tra GameState.AllowInput
			if (blockOnInputDisabled && !GameState.AllowInput)
			{
				return false;
			}

			// Kiểm tra cooldown
			if (interactionCooldown > 0f && Time.time - _lastInteractionTime < interactionCooldown)
			{
				return false;
			}

			// Kiểm tra animator đang chạy animation
			if (blockOnAnimation && animatorRef != null)
			{
				if (animatorRef.IsInTransition(0))
				{
					return false;
				}
			}

			// Có thể thêm các điều kiện khác: đang bẫy, đang dialogue, etc.
			// if (IsTrapped()) return false;
			// if (IsInDialogue()) return false;

			return true;
		}
	}
}


