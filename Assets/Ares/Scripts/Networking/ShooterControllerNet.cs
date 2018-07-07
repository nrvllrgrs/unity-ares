using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	public class ShooterControllerNet : NetworkBehaviour, IShooterController
	{
		#region Variables

		[SerializeField]
		private ShooterControllerData m_data;

		#endregion

		#region Properties

		public ShooterControllerData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

		#endregion

		#region Methods

		private void Reset()
		{
			data = new ShooterControllerData(this);
		}

		public override void OnStartAuthority()
		{
			data.Initialize();
		}

		private void Update()
		{
			if (!hasAuthority)
				return;

			data.Tick();
		}

		#endregion

		#region Commands



		#endregion

		#region Rpc



		#endregion
	}
}