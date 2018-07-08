using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Data
{
	public class HealthChangedEvent : UnityEvent<float, float, GameObject>
	{ }

	[System.Serializable]
	public class HealthData : AresData, ICopyable<HealthData>
	{
		#region Variables

		/// <summary>
		/// Maximum health
		/// </summary>
		[Tooltip("Maximum health")]
		public float maxHealth;

		[SerializeField]
		[Tooltip("Current health")]
		private float m_health;

		/// <summary>
		/// Indicates whether gameObject is destroyed when health is zero
		/// </summary>
		[Tooltip("Indicates whether gameObject is destroyed when health is zero")]
		public bool destroyOnKilled;
		
		/// <summary>
		/// Seconds before regeneration begins
		/// </summary>
		[Tooltip("Seconds before regeneration begins")]
		public float regenerationDelay;

		/// <summary>
		/// Health regenerated per second
		/// </summary>
		[Tooltip("Health regenerated per second")]
		public float regenerationRate;

		/// <summary>
		/// Seconds of invulnerability after taking damage
		/// </summary>
		[Tooltip("Seconds of invulnerability after receiving damage")]
		public float invulnerabilityTime;

		private Coroutine m_regenerationThread, m_invulnerabilityThread;

		#endregion

		#region Events

		public UnityEvent onHealthChanged = new UnityEvent();
		public UnityEvent onKilled = new UnityEvent();

		#endregion

		#region Properties

		public virtual float health
		{
			get { return m_health; }
			private set { m_health = value; }
		}

		public bool isInvulnerable { get; private set; }

		#endregion

		#region Constructors

		public HealthData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void CopyTo(HealthData other)
		{
			other.maxHealth = maxHealth;
			other.health = health;
			other.destroyOnKilled = destroyOnKilled;
			other.regenerationDelay = regenerationDelay;
			other.regenerationRate = regenerationRate;
			other.invulnerabilityTime = invulnerabilityTime;
		}

		public void Damage(DamageActionInfo info)
		{
			float value = info.value;
			value = Mathf.Clamp(value, 0f, maxHealth);

			// TODO: Check faction for friendly-fire

			// No change, or attempt to lose health while invulnerable
			if (health == value || (value < health && isInvulnerable))
				return;

			// User lost health...
			if (value < health && regenerationRate > 0f)
			{
				RunInvulnerabilityThread();
				RunRegenerationThread();
			}

			// User gained health...
			else if (value > health && regenerationRate < 0f)
			{
				RunRegenerationThread();
			}

			m_health = value;

			onHealthChanged.Invoke();
			if (m_health == 0f)
			{
				onKilled.Invoke();

				if (destroyOnKilled)
				{
					Object.Destroy(owner.gameObject);
				}
			}
		}

		private void RunRegenerationThread()
		{
			if (m_regenerationThread != null)
			{
				owner.StopCoroutine(m_regenerationThread);
			}
			m_regenerationThread = owner.StartCoroutine(RegenerationThread());
		}

		private IEnumerator RegenerationThread()
		{
			if (regenerationDelay > 0f)
			{
				yield return new WaitForSeconds(regenerationDelay);
			}

			while (true)
			{
				yield return new WaitForSeconds(1f / regenerationRate);
				health += 1f;

				if (health == maxHealth)
					break;
			}

			m_regenerationThread = null;
		}

		private void RunInvulnerabilityThread()
		{
			if (invulnerabilityTime == 0f)
				return;

			if (m_invulnerabilityThread != null)
			{
				owner.StopCoroutine(m_invulnerabilityThread);
			}

			m_invulnerabilityThread = owner.StartCoroutine(InvulnerabilityThread());
		}

		private IEnumerator InvulnerabilityThread()
		{
			isInvulnerable = true;
			yield return new WaitForSeconds(invulnerabilityTime);
			isInvulnerable = false;

			m_invulnerabilityThread = null;
		}

		#endregion
	}
}
