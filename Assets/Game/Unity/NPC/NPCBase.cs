using UnityEngine;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
	/// <summary>
	/// NPC cơ sở - có thể gắn lên GameObject. Dữ liệu đọc từ NPCDefinitionSO.
	/// </summary>
	public abstract class NPCBase : MonoBehaviour
	{
		[Header("Definition")]
		[SerializeField] protected NPCDefinitionSO definition;

		public virtual void Initialize(NPCDefinitionSO def)
		{
			definition = def;
		}

		/// <summary>
		/// Gọi khi người chơi tương tác (E hoặc click)
		/// </summary>
		public abstract void Interact(PlayerId playerId);
	}
}


