using UnityEngine;

namespace TheGreenMemoir.Unity.Input.Actions
{
	[CreateAssetMenu(fileName = "UIToggleAction", menuName = "Game/Actions/UI Toggle", order = 22)]
	public class UIToggleSO : TheGreenMemoir.Unity.Input.ActionSOBase
	{
		public GameObject targetUI;

		public override void Execute(TheGreenMemoir.Unity.Input.ActionContext context)
		{
			if (targetUI == null) return;
			targetUI.SetActive(!targetUI.activeSelf);
		}
	}
}


