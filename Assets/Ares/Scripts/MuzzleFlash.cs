using UnityEngine;

namespace Ares
{
	[RequireComponent(typeof(IShooterController))]
	public class MuzzleFlash : MonoBehaviour
	{
		#region Variables

		public ParticleSystem muzzleFlash;

		private IShooterController m_controller;

		#endregion

		#region Properties

		public IShooterController controller
		{
			get
			{
				if (m_controller == null)
				{
					m_controller = GetComponent<IShooterController>();
				}
				return m_controller;
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			controller.data.onShotFired.AddListener(() =>
			{
				muzzleFlash.Emit(1);
			});
		}

		#endregion
	}
}