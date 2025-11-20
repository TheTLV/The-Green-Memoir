using UnityEngine;

namespace TheGreenMemoir.Unity.Input.Actions
{
	[CreateAssetMenu(fileName = "SkillAction", menuName = "Game/Actions/Skill", order = 23)]
	public class SkillSO : TheGreenMemoir.Unity.Input.ActionSOBase
	{
		[TextArea]
		public string skillDescription = "Demo skill";

		public override void Execute(TheGreenMemoir.Unity.Input.ActionContext context)
		{
			Debug.Log($"Skill executed: {skillDescription}");
			// TODO: implement VFX/SFX later
		}
	}
}


