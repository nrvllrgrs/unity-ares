using System.Collections;
using UnityEngine;

namespace Ares
{
	public class ShooterCharge : ShooterBlocker
	{
		#region Variables

		/// <summary>
		/// Seconds before controller can shoot
		/// </summary>
		public float maxCharge;

		/// <summary>
		/// Indicates whether charge is reset when "Fire" ends (otherwise, value charges down)
		/// </summary>
		public bool resetOnEndFire;

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

		#region Methods

		protected virtual void Awake()
		{
			controller.onBeginFire.AddListener(OnBeginFire);
			controller.onEndFire.AddListener(OnEndFire);
		}

		private void OnBeginFire()
		{
			if (m_thread != null)
			{
				StopCoroutine(m_thread);
			}

			m_thread = StartCoroutine(ChargeUpThread());
		}

		private void OnEndFire()
		{
			if (m_thread != null)
			{
				StopCoroutine(m_thread);
			}

			if (resetOnEndFire)
			{
				charge = 0;
			}
			else
			{
				m_thread = StartCoroutine(ChargeDownThread());
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
