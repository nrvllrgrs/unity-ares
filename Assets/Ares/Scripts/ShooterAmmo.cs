using UnityEngine;
using Ares.Data;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class ShooterAmmo : AresMonoBehaviour<ShooterAmmoData>, IShooterAmmo, IShooterBlocker
	{
		#region Properties

		public ShooterBlockerData blockerData
		{
			get { return data; }
		}

		#endregion

		#region Methods

		protected override void Reset()
		{
			data = new ShooterAmmoData(this);
		}

		#endregion
	}
}
