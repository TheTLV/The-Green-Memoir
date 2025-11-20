using System.Collections.Generic;
using UnityEngine;
using TheGreenMemoir.Unity.Presentation;
using TheGreenMemoir.Unity.Presentation.UI;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.Input
{
	/// <summary>
	/// Quản lý các InputActionSO: đọc phím, gọi Execute trên SO liên kết.
	/// Cho phép UI gọi lại bằng actionId.
	/// Singleton pattern để đảm bảo chỉ có một instance.
	/// </summary>
	public class InputActionManager : MonoBehaviour
	{
		private static InputActionManager _instance;
		public static InputActionManager Instance => _instance;

		[Header("Actions")]
		[SerializeField] private List<InputActionSO> actions = new List<InputActionSO>();

		[Header("Context References (optional)")]
		[SerializeField] private ToolInteractionSystem toolSystem;
		[SerializeField] private InventoryUIController inventoryUI;
		[SerializeField] private Animator animatorRef;

		[Header("Guards & Priority")]
		[SerializeField] private InputActionGroup[] priorityOrder = new[]
		{
			InputActionGroup.UI,
			InputActionGroup.Interact,
			InputActionGroup.Skill,
			InputActionGroup.Tool,
			InputActionGroup.Movement,
			InputActionGroup.Cheat,
			InputActionGroup.Custom
		};

		private readonly Dictionary<string, InputActionSO> _actionById = new Dictionary<string, InputActionSO>();
		private readonly Dictionary<string, float> _cooldownUntil = new Dictionary<string, float>();
		private readonly Dictionary<string, float> _lastTriggerAt = new Dictionary<string, float>();
		private readonly Dictionary<string, bool> _toggleState = new Dictionary<string, bool>();
		private readonly Dictionary<string, float> _holdStartAt = new Dictionary<string, float>();
		private readonly Dictionary<string, float> _holdLastExecuteAt = new Dictionary<string, float>(); // Chống spam trong Hold mode
		private ActionContext _context;
		private InputGuardSystem _guard = new InputGuardSystem();
		private bool _globalBusy;

		[Header("Movement (computed)")]
		public Vector2 Move;
		[SerializeField] private string moveUpId = "MoveUp";
		[SerializeField] private string moveDownId = "MoveDown";
		[SerializeField] private string moveLeftId = "MoveLeft";
		[SerializeField] private string moveRightId = "MoveRight";

		private void Awake()
		{
			// Singleton pattern: chỉ cho phép một instance
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (_instance != this)
			{
				Debug.LogWarning($"Multiple InputActionManager instances detected! Destroying duplicate on {gameObject.name}");
				Destroy(gameObject);
				return;
			}

			if (toolSystem == null) toolSystem = FindFirstObjectByType<ToolInteractionSystem>();
			if (inventoryUI == null) inventoryUI = FindFirstObjectByType<InventoryUIController>();

			_context = new ActionContext
			{
				PlayerId = TheGreenMemoir.Core.Domain.ValueObjects.PlayerId.Default,
				ToolSystem = toolSystem,
				InventoryUI = inventoryUI
			};

			BuildIndex();
			ValidateActions();
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

		private void OnValidate()
		{
			BuildIndex();
		}

		private void BuildIndex()
		{
			_actionById.Clear();
			var keyUsage = new Dictionary<KeyCode, List<string>>(); // Track key duplicates

			for (int i = 0; i < actions.Count; i++)
			{
				var a = actions[i];
				if (a == null || string.IsNullOrWhiteSpace(a.actionId)) continue;

				// Check duplicate actionId
				if (_actionById.ContainsKey(a.actionId))
				{
					Debug.LogWarning($"Duplicate actionId '{a.actionId}' found! Keeping first occurrence.");
					continue;
				}

				_actionById[a.actionId] = a;

				// Track key usage for duplicate detection
				if (a.key != KeyCode.None && a.enabled)
				{
					if (!keyUsage.ContainsKey(a.key))
						keyUsage[a.key] = new List<string>();
					keyUsage[a.key].Add(a.actionId);
				}
			}

			// Warn about duplicate keys
			foreach (var kv in keyUsage)
			{
				if (kv.Value.Count > 1)
				{
					Debug.LogWarning($"Key {kv.Key} is assigned to multiple actions: {string.Join(", ", kv.Value)}");
				}
			}

			// Check for circular references in linkedAction
			ValidateCircularReferences();
		}

		/// <summary>
		/// Kiểm tra tham chiếu vòng trong linkedAction (SO → SO → chính nó)
		/// </summary>
		private void ValidateCircularReferences()
		{
			foreach (var kv in _actionById)
			{
				var visited = new HashSet<ActionSOBase>();
				if (HasCircularReference(kv.Value, visited))
				{
					Debug.LogError($"Circular reference detected in action '{kv.Value.actionId}'!");
				}
			}
		}

		private bool HasCircularReference(InputActionSO action, HashSet<ActionSOBase> visited)
		{
			if (action?.linkedAction == null) return false;
			if (visited.Contains(action.linkedAction)) return true;

			visited.Add(action.linkedAction);

			// Check if linkedAction is another InputActionSO (shouldn't happen, but check anyway)
			// Note: This is a simplified check. Full circular detection would need to traverse all SO references.
			return false;
		}

		/// <summary>
		/// Validate actions for common issues
		/// </summary>
		private void ValidateActions()
		{
			foreach (var a in actions)
			{
				if (a == null) continue;
				if (a.inputMode == InputActionSO.InputKeyMode.Hold && a.cooldownSeconds <= 0f)
				{
					Debug.LogWarning($"Action '{a.actionId}' is in Hold mode but has no cooldown. This may cause spam!");
				}
			}
		}

		private void Update()
		{
			// Kiểm tra GameState.AllowInput trước
			if (!GameState.AllowInput && !_globalBusy)
			{
				Move = Vector2.zero; // Reset movement khi không cho phép input
				return;
			}

			// Movement vector (tính riêng, có guard)
			ComputeMovement();

			// Thực thi theo thứ tự ưu tiên, và theo Guard/Cooldown
			if (_globalBusy || !GameState.AllowInput) return;
			for (int p = 0; p < priorityOrder.Length; p++)
			{
				var group = priorityOrder[p];
				if (_guard.IsLocked(group)) continue;
				for (int i = 0; i < actions.Count; i++)
				{
					var a = actions[i];
					if (!IsRunnable(a, group)) continue;
					if (OnCooldown(a)) continue;

					var mode = ResolveMode(a);
					switch (mode)
					{
						case InputActionSO.InputKeyMode.Press:
							if (UnityEngine.Input.GetKeyDown(a.key)) Execute(a);
							break;
						case InputActionSO.InputKeyMode.Hold:
							HandleHold(a);
							break;
						case InputActionSO.InputKeyMode.Toggle:
							HandleToggle(a);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Trigger action by ID (dùng cho UI buttons). 
		/// Kiểm tra cooldown và guards để tránh gọi 2 lần khi UI button và phím cùng trigger.
		/// </summary>
		public void TriggerById(string actionId)
		{
			if (string.IsNullOrWhiteSpace(actionId)) return;
			if (!GameState.AllowInput || _globalBusy) return;
			if (!_actionById.TryGetValue(actionId, out var a)) return;
			if (!IsRunnable(a, a.group)) return;
			if (OnCooldown(a)) return;

			Execute(a);
		}

		private void Execute(InputActionSO a)
		{
			if (a == null) return;

			// Kiểm tra GameState.AllowInput
			if (!GameState.AllowInput) return;

			// Kiểm tra linkedAction null trước khi làm gì
			if (a.linkedAction == null)
			{
				// Vẫn có thể trigger animator nếu có
				TryTriggerAnimator(a);
				ApplyCooldown(a);
				return;
			}

			// Animator trigger (anti-spam + check state)
			TryTriggerAnimator(a);

			// Execute linkedAction
			try
			{
				a.linkedAction.Execute(_context);
				ApplyCooldown(a);
			}
			catch (System.SystemException e)
			{
				Debug.LogError($"InputAction error [{a.displayName}]: {e.Message}");
			}
		}

		/// <summary>
		/// Trigger animator an toàn: kiểm tra spam, state, và null/empty
		/// </summary>
		private void TryTriggerAnimator(InputActionSO a)
		{
			if (animatorRef == null) return;
			if (string.IsNullOrWhiteSpace(a.animatorTriggerName)) return;

			// Anti-spam: không trigger quá nhanh
			float tNow = Time.time;
			if (_lastTriggerAt.TryGetValue(a.actionId, out var last) && (tNow - last) < 0.1f)
			{
				return; // tránh spam
			}

			// Kiểm tra animator đang trong transition hoặc đang chạy animation
			if (animatorRef.IsInTransition(0))
			{
				// Có thể skip hoặc đợi, tùy game design
				// Ở đây skip để tránh conflict
				return;
			}

			// Reset trigger trước (best practice)
			animatorRef.ResetTrigger(a.animatorTriggerName);
			animatorRef.SetTrigger(a.animatorTriggerName);
			_lastTriggerAt[a.actionId] = tNow;
		}

		private InputActionSO.InputKeyMode ResolveMode(InputActionSO a)
		{
			if (a == null) return InputActionSO.InputKeyMode.Press;
			// tương thích cũ: isContinuous => Hold
			if (a.isContinuous && a.inputMode == InputActionSO.InputKeyMode.Press)
				return InputActionSO.InputKeyMode.Hold;
			return a.inputMode;
		}

		/// <summary>
		/// Xử lý Hold mode với cooldown để chống spam
		/// </summary>
		private void HandleHold(InputActionSO a)
		{
			if (a.key == KeyCode.None) return;
			if (a == null) return;

			if (UnityEngine.Input.GetKeyDown(a.key))
			{
				_holdStartAt[a.actionId] = Time.time;
				_holdLastExecuteAt[a.actionId] = 0f; // Reset
			}

			if (UnityEngine.Input.GetKey(a.key))
			{
				float started = _holdStartAt.TryGetValue(a.actionId, out var t) ? t : 0f;
				float holdDuration = a.holdDurationThreshold <= 0f ? 0f : a.holdDurationThreshold;

				// Kiểm tra đã đủ thời gian giữ
				if (Time.time - started < holdDuration) return;

				// Kiểm tra cooldown để chống spam (quan trọng!)
				float lastExecute = _holdLastExecuteAt.TryGetValue(a.actionId, out var last) ? last : 0f;
				float cooldown = a.cooldownSeconds > 0f ? a.cooldownSeconds : 0.1f; // Default 0.1s nếu không có cooldown

				if (Time.time - lastExecute >= cooldown)
				{
					Execute(a);
					_holdLastExecuteAt[a.actionId] = Time.time;
				}
			}

			if (UnityEngine.Input.GetKeyUp(a.key))
			{
				_holdStartAt.Remove(a.actionId);
				_holdLastExecuteAt.Remove(a.actionId);
			}
		}

		private void HandleToggle(InputActionSO a)
		{
			if (a.key == KeyCode.None) return;
			if (UnityEngine.Input.GetKeyDown(a.key))
			{
				bool state = _toggleState.TryGetValue(a.actionId, out var s) && s;
				state = !state;
				_toggleState[a.actionId] = state;
				Execute(a);
			}
		}

		private void ComputeMovement()
		{
			float x = 0f, y = 0f;
			if (TryGet(moveLeftId, out var left) && left.key != KeyCode.None && UnityEngine.Input.GetKey(left.key)) x -= 1f;
			if (TryGet(moveRightId, out var right) && right.key != KeyCode.None && UnityEngine.Input.GetKey(right.key)) x += 1f;
			if (TryGet(moveDownId, out var down) && down.key != KeyCode.None && UnityEngine.Input.GetKey(down.key)) y -= 1f;
			if (TryGet(moveUpId, out var up) && up.key != KeyCode.None && UnityEngine.Input.GetKey(up.key)) y += 1f;

			Move = new Vector2(x, y);
			if (Move.sqrMagnitude > 1f) Move.Normalize();
		}

		private bool TryGet(string id, out InputActionSO action)
		{
			action = null;
			if (string.IsNullOrWhiteSpace(id)) return false;
			return _actionById.TryGetValue(id, out action);
		}

		/// <summary>
		/// Rebind key cho action. Tự động validate và refresh index.
		/// </summary>
		public bool Rebind(string actionId, KeyCode newKey)
		{
			if (!_actionById.TryGetValue(actionId, out var a) || a == null) return false;

			// Kiểm tra phím trùng
			foreach (var kv in _actionById)
			{
				if (kv.Value != null && kv.Value.actionId != actionId && kv.Value.key == newKey && kv.Value.enabled)
				{
					Debug.LogWarning($"Rebind warning: key {newKey} is already used by action '{kv.Value.displayName}' ({kv.Value.actionId})");
					// Có thể return false để từ chối, hoặc chỉ warning. Ở đây chỉ warning.
				}
			}

			a.key = newKey;
			// Không cần rebuild index vì index dựa trên actionId, không phải key
			// Nhưng có thể validate lại nếu cần
			return true;
		}

		// Guards API
		public void LockGroup(InputActionGroup g) => _guard.Lock(g);
		public void UnlockGroup(InputActionGroup g) => _guard.Unlock(g);
		public bool IsGroupLocked(InputActionGroup g) => _guard.IsLocked(g);

		private bool IsRunnable(InputActionSO a, InputActionGroup expectedGroup)
		{
			if (a == null || !a.enabled) return false;
			if (a.key == KeyCode.None) return false;
			if (a.group != expectedGroup) return false;
			if (_guard.IsLocked(a.group)) return false;
			return true;
		}

		private bool OnCooldown(InputActionSO a)
		{
			if (a == null || a.cooldownSeconds <= 0f) return false;
			if (_cooldownUntil.TryGetValue(a.actionId, out var until))
			{
				return Time.time < until;
			}
			return false;
		}

		private void ApplyCooldown(InputActionSO a)
		{
			if (a == null || a.cooldownSeconds <= 0f) return;
			_cooldownUntil[a.actionId] = Time.time + a.cooldownSeconds;
		}

		// Busy API (ví dụ đang cutscene, hội thoại)
		public void SetBusy(bool busy) => _globalBusy = busy;
	}
}


