using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Data
{
	[System.Serializable]
	public class ShooterHeatData : ShooterBlockerData, ICopyable<ShooterHeatData>
	{
		#region Variables

		/// <summary>
		/// Maximum allowed heat before overheating
		/// </summary>
		[Tooltip("Maximum allowed heat before overheating")]
		public float maxHeat;

		/// <summary>
		/// Heat per shot
		/// </summary>
		public float heatPerShot;

		/// <summary>
		/// Seconds to wait before heat loss begins while not overheated
		/// </summary>
		[Tooltip("Seconds to wait before heat loss begins while not overheated")]
		public float cooldownDelay;

		/// <summary>
		/// Heat loss per second while not overheated
		/// </summary>
		[Tooltip("Heat loss per second while not overheated")]
		public float cooldownRate;

		/// <summary>
		/// Seconds to wait before heat loss begins while overheated
		/// </summary>
		[Tooltip("Seconds to wait before heat loss begins while overheated")]
		public float overheatDelay;

		/// <summary>
		/// Heat loss per second while overheated
		/// </summary>
		[Tooltip("Heat loss per second while overheated")]
		public float overheatRate;

		private float m_heat;
		private bool m_isOverheated;
		private Coroutine m_thread;

		#endregion

		#region Events

		public UnityEvent onHeatChanged = new UnityEvent();
		public UnityEvent onBeginOverheat = new UnityEvent();
		public UnityEvent onEndOverheat = new UnityEvent();

		#endregion

		#region Properties

		public override bool canFire
		{
			get { return !isOverheated; }
		}

		public float heat
		{
			get { return m_heat; }
			private set
			{
				value = Mathf.Clamp(value, 0f, maxHeat);
				if (heat == value)
					return;

				m_heat = value;
				onHeatChanged.Invoke();
			}
		}

		public float percent
		{
			get { return heat / maxHeat; }
		}

		public bool isOverheated
		{
			get { return m_isOverheated; }
			private set
			{
				if (isOverheated == value)
					return;

				m_isOverheated = value;
				if (isOverheated)
				{
					onBeginOverheat.Invoke();
				}
				else
				{
					onEndOverheat.Invoke();
				}
			}
		}

		#endregion

		#region Constructors

		public ShooterHeatData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void CopyTo(ShooterHeatData other)
		{
			other.maxHeat = maxHeat;
			other.heatPerShot = heatPerShot;
		}

		public override void Initialize()
		{
			controller.data.onShotFired.AddListener(OnShotFired);
		}

		private void OnShotFired()
		{
			heat += controller.data.isContinuous
				? heatPerShot * Time.deltaTime
				: heatPerShot;

			if (heat == maxHeat)
			{
				m_thread = owner.StartCoroutine(OverheatThread());
			}
			else
			{
				// Stop cooldown thread, assuming it exists
				if (m_thread != null)
				{
					owner.StopCoroutine(m_thread);
				}

				m_thread = owner.StartCoroutine(CooldownThread());
			}
		}

		private IEnumerator CooldownThread()
		{
			if (cooldownDelay > 0f)
			{
				yield return new WaitForSeconds(cooldownDelay);
			}

			while (true)
			{
				yield return new WaitForEndOfFrame();
				heat -= cooldownRate * Time.deltaTime;

				if (heat == 0f)
				{
					break;
				}
			}

			m_thread = null;
		}

		private IEnumerator OverheatThread()
		{
			isOverheated = true;

			if (overheatDelay > 0f)
			{
				yield return new WaitForSeconds(overheatDelay);
			}

			while (true)
			{
				yield return new WaitForEndOfFrame();
				heat -= overheatRate * Time.deltaTime;

				if (heat == 0f)
				{
					break;
				}
			}

			isOverheated = false;
			m_thread = null;
		}

		#endregion
	}
}
