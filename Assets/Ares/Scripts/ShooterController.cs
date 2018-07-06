using UnityEngine;
using Ares.Data;

namespace Ares
{
	public class ShooterController : AresMonoBehaviour<ShooterControllerData>, IShooterController
	{
		#region Methods

		protected override void Reset()
		{
			data = new ShooterControllerData(this);
		}

		private void Update()
		{
			data.Tick();
		}

		#endregion
	}
}