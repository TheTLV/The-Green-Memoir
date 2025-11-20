using UnityEngine;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
	public enum NPCType
	{
		Generic,
		Shop,
		Quest,
		Enemy
	}

	[CreateAssetMenu(fileName = "NPCDefinition", menuName = "Game/NPC Definition", order = 10)]
	public class NPCDefinitionSO : ScriptableObject
	{
		[Header("Identity")]
		public string npcId = "npc_default";
		public string displayName = "NPC";
		public NPCType type = NPCType.Generic;
		public Sprite portrait;

		[Header("Shop (optional)")]
		public bool isShop;
		public int baseBuyMultiplier = 100; // %
		public int baseSellMultiplier = 100; // %

		[Header("Friendship (optional)")]
		[Tooltip("Cấu hình độ thân mật với NPC này. Chỉ dùng nếu GameSettingsSO.enableNPCFriendship = true")]
		public NPCFriendshipSO friendshipConfig;
	}
}


