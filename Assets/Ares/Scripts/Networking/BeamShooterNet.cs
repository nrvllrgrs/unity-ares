using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	[RequireComponent(typeof(ShooterControllerNet))]
	public class BeamShooterNet : NetworkBehaviour, IShooter<BeamShooterData>, IShooter
	{
		#region Variables

		[SerializeField]
		private BeamShooterData m_data;

		#endregion

		#region Properties

		public BeamShooterData data
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
			data = new BeamShooterData(this);
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
