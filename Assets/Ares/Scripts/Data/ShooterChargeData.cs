using UnityEngine;
using System.Collections;

namespace Ares.Data
{
	[System.Serializable]
	public class ShooterChargeData : ShooterBlockerData
	{
		#region Variables

		/// <summary>
		/// Seconds before controller can shoot
		/// </summary>
		[Tooltip("Seconds before controller can shoot")]
		public float maxCharge;

		/// <summary>
		/// Indicates whether charge is reset when "Fire" ends (otherwise, value charges down)
		/// </summary>
		[Tooltip("Indicates whether charge is reset when \"Fire\" ends (otherwise, value charges down)")]
		public bool resetOnEndFire;

		// TODO: Add functionality to fire on partial charge (similar to Plasma Pistol in Halo)

		private Coroutine m_thread;
		private float m_charge;

		#endregion

		#region Properties

		public override bool canFire
		{
			get { return charge == maxCharge; }
		}

		public float charge
		{
			get { return m_charge; }
			private set
			{
				value = Mathf.Clamp(value, 0f, maxCharge);
				if (charge == value)
					return;

				m_charge = value;
			}
		}

		public float percent
		{
			get { return charge / maxCharge; }
		}

		#endregion

		#region Constructors

		public ShooterChargeData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public override void Initialize()
		{
			controller.data.onBeginFire.AddListener(OnBeginFire);
			controller.data.onEndFire.AddListener(OnEndFire);
		}

		private void OnBeginFire()
		{
			if (m_thread != null)
			{
				owner.StopCoroutine(m_thread);
			}

			m_thread = owner.StartCoroutine(ChargeUpThread());
		}

		private void OnEndFire()
		{
			if (m_thread != null)
			{
				owner.StopCoroutine(m_thread);
			}

			if (resetOnEndFire)
			{
				charge = 0;
			}
			else
			{
				m_thread = owner.StartCoroutine(ChargeDownThread());
			}
		}

		private IEnumerator ChargeUpThread()
		{
			// Spin up
			while (true)
			{
				yield return new WaitForEndOfFrame();

				charge += Time.deltaTime;
				if (charge == maxCharge)
					break;
			}
		}

		private IEnumerator ChargeDownThread()
		{
			// Spin down
			while (true)
			{
				yield return new WaitForEndOfFrame();

				charge -= Time.deltaTime;
				if (charge == 0)
					break;
			}
		}

		#endregion
	}
}
