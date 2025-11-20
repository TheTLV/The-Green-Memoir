using UnityEngine;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
	/// <summary>
	/// NPC kẻ địch (demo) - hiện chỉ log, có thể mở combat sau
	/// </summary>
	public class EnemyNPC : NPCBase
	{
		public override void Interact(PlayerId playerId)
		{
			Debug.Log($"EnemyNPC {definition?.displayName} interacted. (TODO: Combat)");
		}
	}
}


