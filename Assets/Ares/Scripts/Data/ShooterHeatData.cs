using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Data
{
	[System.Serializable]
	public class ShooterHeatData : ShooterBlockerData
	{
		#region Variables

		public float maxHeat;
		public float heatPerShot;

		public float cooldownDelay;
		public float cooldownRate;

		public float overheatDelay;
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
