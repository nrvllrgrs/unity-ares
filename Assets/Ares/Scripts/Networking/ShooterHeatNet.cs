using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	[RequireComponent(typeof(ShooterControllerNet))]
	public class ShooterHeatNet : NetworkBehaviour, IShooterHeat, IShooterBlocker
	{
		#region Variables

		[SerializeField]
		private ShooterHeatData m_data;

		#endregion

		#region Properties

		public ShooterHeatData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

		public ShooterBlockerData blockerData
		{
			get { return data; }
		}

		#endregion

		#region Methods

		private void Reset()
		{
			data = new ShooterHeatData(this);
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
