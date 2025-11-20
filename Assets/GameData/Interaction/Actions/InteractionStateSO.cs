using UnityEngine;

namespace TheGreenMemoir.Unity.Interaction
{
	/// <summary>
	/// Định nghĩa một trạng thái có thể gán cho object (vd: Normal, Hoed, Watered, Seeded,...)
	/// </summary>
	[CreateAssetMenu(fileName = "InteractionState", menuName = "Game/Interaction/State", order = 40)]
	public class InteractionStateSO : ScriptableObject
	{
		public string stateId = "state_id"; // unique per graph
		[TextArea] public string description;
	}
}


