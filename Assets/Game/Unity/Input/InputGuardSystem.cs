using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Input
{
	/// <summary>
	/// Hệ thống khóa nhập liệu theo nhóm (Movement, Skill, Tool, UI, Interact, ...)
	/// </summary>
	public class InputGuardSystem
	{
		private readonly HashSet<InputActionGroup> _locked = new HashSet<InputActionGroup>();

		public void Lock(InputActionGroup group)
		{
			_locked.Add(group);
		}

		public void Unlock(InputActionGroup group)
		{
			_locked.Remove(group);
		}

		public bool IsLocked(InputActionGroup group)
		{
			return _locked.Contains(group);
		}
	}
}


