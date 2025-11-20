using UnityEngine;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.Input
{
	public abstract class ActionSOBase : ScriptableObject
	{
		[Header("Audio (optional)")]
		[Tooltip("Âm thanh phát khi thực hiện action này")]
		public AudioClipSO audioClip;

		public abstract void Execute(ActionContext context);

		/// <summary>
		/// Phát âm thanh nếu có
		/// </summary>
		protected void PlayAudio(Vector3? position = null)
		{
			if (audioClip != null)
			{
				audioClip.Play(position);
			}
		}
	}
}


