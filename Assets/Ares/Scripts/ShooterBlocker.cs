using UnityEngine;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public abstract class ShooterBlocker : MonoBehaviour
	{
		#region Variables

		private ShooterController m_controller;

		#endregion

		#region Properties

		public ShooterController controller
		{
			get
			{
				if (m_controller == null)
				{
					m_controller = GetComponent<ShooterController>();
				}
				return m_controller;
			}
		}

		public abstract bool canFire { get; }

		#endregion
	}
}
