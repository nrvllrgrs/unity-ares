using UnityEngine;
using Ares.Data;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class BeamShooter : AresMonoBehaviour<BeamShooterData>, IShooter<BeamShooterData>, IShooter
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
			data = new BeamShooterData(this);
		}

		#endregion
	}
}
