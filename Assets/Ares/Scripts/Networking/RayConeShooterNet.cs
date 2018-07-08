using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	[RequireComponent(typeof(ShooterControllerNet))]
	public class RayConeShooterNet : NetworkBehaviour, IShooter<RayConeShooterData>, IShooter
	{
		#region Variables

		[SerializeField]
		private RayConeShooterData m_data;

		#endregion

		#region Properties

		public RayConeShooterData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

		public ShooterData shooterData
		{
			get { return data; }
		}

		#endregion

		#region Methods

		private void Reset()
		{
			data = new RayConeShooterData(this);
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