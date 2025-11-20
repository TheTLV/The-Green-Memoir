using UnityEngine;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Input.Actions
{
	[CreateAssetMenu(fileName = "ToolAction", menuName = "Game/Actions/Tool", order = 21)]
	public class ToolSO : TheGreenMemoir.Unity.Input.ActionSOBase
	{
		public ToolActionType tool = ToolActionType.Plow;

		public override void Execute(TheGreenMemoir.Unity.Input.ActionContext context)
		{
			if (context?.ToolSystem == null) return;
			context.ToolSystem.SetTool(tool);
			
			// Phát âm thanh nếu có
			PlayAudio();

			Debug.Log($"Tool action executed: {tool}");
		}
	}
}


