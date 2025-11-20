using UnityEngine;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Unity.Presentation.UI;

namespace TheGreenMemoir.Unity.NPC
{
	/// <summary>
	/// NPC cửa hàng - mở UI shop khi tương tác
	/// </summary>
	public class ShopNPC : NPCBase
	{
		[SerializeField] private NPCShopUI shopUI;

		private void Awake()
		{
			if (shopUI == null)
				shopUI = FindFirstObjectByType<NPCShopUI>();
		}

		public override void Interact(PlayerId playerId)
		{
			if (definition == null || shopUI == null)
				return;
			shopUI.ShowShop(new NPCId(definition.npcId));
		}
	}
}


