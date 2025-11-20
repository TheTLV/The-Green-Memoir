using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheGreenMemoir.Unity.Input
{
	/// <summary>
	/// UI cực đơn giản để đổi phím runtime.
	/// </summary>
	public class InputSettingsUI : MonoBehaviour
	{
		[SerializeField] private InputActionManager manager;
		[SerializeField] private Transform listContainer;
		[SerializeField] private GameObject rowPrefab; // Row có Text(Name) + Button(Key)

		private bool _waitingForKey;
		private InputActionSO _pendingAction;

		private void Awake()
		{
			if (manager == null) manager = FindFirstObjectByType<InputActionManager>();
			BuildList();
		}

		private void Update()
		{
			if (_waitingForKey && UnityEngine.Input.anyKeyDown)
			{
				// Tìm key vừa nhấn
				var newKey = DetectPressedKey();
				if (newKey != KeyCode.None && _pendingAction != null)
				{
					manager.Rebind(_pendingAction.actionId, newKey);
					_waitingForKey = false;
					_pendingAction = null;
					RebuildList();
				}
			}
		}

		private void BuildList()
		{
			RebuildList();
		}

		private void RebuildList()
		{
			if (listContainer == null || rowPrefab == null || manager == null) return;
			foreach (Transform c in listContainer) Destroy(c.gameObject);
			var field = typeof(InputActionManager).GetField("actions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var list = field?.GetValue(manager) as List<InputActionSO>;
			if (list == null) return;
			for (int i = 0; i < list.Count; i++)
			{
				var a = list[i]; if (a == null) continue;
				var row = Instantiate(rowPrefab, listContainer);
				var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
				if (texts.Length > 0) texts[0].text = a.displayName;
				var btn = row.GetComponentInChildren<Button>(true);
				if (btn != null)
				{
					var lbl = btn.GetComponentInChildren<TextMeshProUGUI>(true);
					if (lbl != null) lbl.text = a.key.ToString();
					btn.onClick.RemoveAllListeners();
					btn.onClick.AddListener(() => BeginRebind(a));
				}
			}
		}

		private void BeginRebind(InputActionSO action)
		{
			_pendingAction = action;
			_waitingForKey = true;
		}

		private KeyCode DetectPressedKey()
		{
			// Duyệt tất cả KeyCode (đủ dùng cho demo)
			foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
			{
				if (UnityEngine.Input.GetKeyDown(k)) return k;
			}
			return KeyCode.None;
		}
	}
}


