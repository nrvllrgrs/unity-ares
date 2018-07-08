using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	[RequireComponent(typeof(ShooterControllerNet))]
	public class ProjectileShooterNet : NetworkBehaviour, IShooter<ProjectileShooterData>, IShooter
	{
		#region Variables

		#region Variables

		public ProjectileNet projectileTemplate;

		#endregion

		[SerializeField]
		private ProjectileShooterData m_data;

		#endregion

		#region Properties

		public ProjectileShooterData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

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

		private void Reset()
		{
			data = new ProjectileShooterData(this);
		}

		public override void OnStartAuthority()
		{
			data.Initialize();
		}

		#endregion

		#region Commands



		#endregion

		#region Rpc



		#endregion
	}
}
