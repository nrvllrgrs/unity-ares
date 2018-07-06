using UnityEngine;
using Ares.Data;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class RayConeShooter : AresMonoBehaviour<RayConeShooterData>, IShooter<RayConeShooterData>, IShooter
	{
		#region Properties

		public ShooterData shooterData
		{
			get { return data; }
		}

		#endregion

		#region Methods

		protected override void Reset()
		{
			data = new RayConeShooterData(this);
		}

		#endregion
	}
}
