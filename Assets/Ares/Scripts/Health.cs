using UnityEngine;
using Ares.Data;

namespace Ares
{
	public class Health : AresMonoBehaviour<HealthData>, IHealth
	{
		#region Methods

		protected override void Reset()
		{
			data = new HealthData(this);
		}

		#endregion

		#region Editor Methods
#if UNITY_EDITOR

		[ContextMenu("Reset Owner")]
		protected override void ResetOwner()
		{
			base.ResetOwner();
		}

#endif
		#endregion
	}
}