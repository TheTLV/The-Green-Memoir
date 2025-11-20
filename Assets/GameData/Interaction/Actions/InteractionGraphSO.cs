using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Interaction
{
	/// <summary>
	/// Biểu đồ tương tác cho một loại object (vd: Tile Đất, Cây, Đá,...)
	/// Chứa danh sách state, action và transitions cấu hình bằng Inspector.
	/// </summary>
	[CreateAssetMenu(fileName = "InteractionGraph", menuName = "Game/Interaction/Graph", order = 43)]
	public class InteractionGraphSO : ScriptableObject
	{
		[Header("States")]
		public List<InteractionStateSO> states = new List<InteractionStateSO>();

		[Header("Actions")]
		public List<InteractionActionSO> actions = new List<InteractionActionSO>();

		[Header("Transitions")]
		public List<InteractionTransitionSO> transitions = new List<InteractionTransitionSO>();

		/// <summary>
		/// Tìm transition hợp lệ từ fromState qua action
		/// </summary>
		public InteractionTransitionSO FindTransition(InteractionStateSO fromState, InteractionActionSO action)
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				var t = transitions[i];
				if (t != null && t.fromState == fromState && t.action == action)
					return t;
			}
			return null;
		}
	}
}


