using UnityEngine;
using Ares.Data;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class RayShooter : AresMonoBehaviour<RayShooterData>, IShooter<RayShooterData>, IShooter
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
			data = new RayShooterData(this);
		}

		#endregion
	}
}
