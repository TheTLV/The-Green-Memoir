using UnityEngine;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
	/// <summary>
	/// NPC nhiệm vụ - hiện placeholder (có thể mở UI quest sau)
	/// </summary>
	public class QuestNPC : NPCBase
	{
		public override void Interact(PlayerId playerId)
		{
			Debug.Log($"QuestNPC {definition?.displayName} interacted. (TODO: Open quest UI)");
		}
	}
}


