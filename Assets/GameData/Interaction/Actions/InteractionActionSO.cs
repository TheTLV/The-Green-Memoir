using UnityEngine;

namespace TheGreenMemoir.Unity.Interaction
{
	/// <summary>
	/// Định nghĩa một hành động (vd: Plow, Water, Plant, Harvest, Mine,...)
	/// </summary>
	[CreateAssetMenu(fileName = "InteractionAction", menuName = "Game/Interaction/Action", order = 41)]
	public class InteractionActionSO : ScriptableObject
	{
		public string actionId = "action_id";
		[TextArea] public string description;
	}
}


