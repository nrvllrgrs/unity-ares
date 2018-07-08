using UnityEngine;
using Ares.Data;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class ProjectileShooter : AresMonoBehaviour<ProjectileShooterData>, IShooter<ProjectileShooterData>, IShooter
	{
		#region Variables

		public Projectile projectileTemplate;

		#endregion

		#region Properties

		public ShooterData shooterData
		{
			get { return data; }
		}

		public ProjectileShooterData projectileShooterData
		{
			get { return data; }
		}

		#endregion

		#region Methods

		protected override void Reset()
		{
			data = new ProjectileShooterData(this);
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
