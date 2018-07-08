using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;

namespace Ares.Networking
{
	public class HealthNet : NetworkBehaviour, IHealth
	{
		#region Variables

		[SerializeField]
		private HealthData m_data;

		#endregion

		#region Properties

		public HealthData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

		#endregion

		#region Methods

		private void Reset()
		{
			data = new HealthData(this);
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