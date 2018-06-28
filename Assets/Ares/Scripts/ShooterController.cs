using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Ares
{
	public class ShooterController : MonoBehaviour
	{
		#region Variables

		/// <summary>
		/// Indicates whether player controls this controller
		/// </summary>
		public bool isPlayerControlled;

		/// <summary>
		/// Indicates whether "Fire" action triggers every frame
		/// </summary>
		public bool isContinuous = false;

		/// <summary>
		/// Indicates whether "Fire" action automatically triggers on button down
		/// </summary>
		public bool isAutoFire = false;

		/// <summary>
		/// Seconds between shots
		/// </summary>
		public float timeBetweenShots;

		/// <summary>
		/// Indicates whether "Fire" action occurs on button down
		/// </summary>
		public bool fireOnButtonDown = true;

		/// <summary>
		/// Name of Fire button
		/// </summary>
		public string fireButton = "Fire1";

		/// <summary>
		/// Indicates whether "Fire" action triggers multiple shots
		/// </summary>
		public bool isBurstFire = false;

		/// <summary>
		/// Seconds between bursts
		/// </summary>
		public float timeBetweenBursts;

		/// <summary>
		/// Number of shots per burst
		/// </summary>
		public int shotsPerBurst;

		private bool m_isFiring;
		private float m_beginFireTime, m_lastShotTime;
		private int m_burstShotCount;
		private Coroutine m_burstThread;

		private GameObject m_carrier;
		private ShooterAmmo m_ammo;
		private ShooterBlocker[] m_blockers;
		private Shooter[] m_shooters;

		#endregion

		#region Events

		public UnityEvent onBeginFire = new UnityEvent();
		public UnityEvent onShotFiring = new UnityEvent();
		public UnityEvent onShotFired = new UnityEvent();
		public UnityEvent onDryFired = new UnityEvent();
		public UnityEvent onEndFire = new UnityEvent();

		#endregion

		#region Properties

		/// <summary>
		/// GameObject that is carrying this weapon
		/// </summary>
		public GameObject carrier
		{
			get { return m_carrier; }
			set
			{
				if (carrier == value)
					return;

				m_carrier = value;
			}
		}

		/// <summary>
		/// Indicates whether this weapon can fire
		/// </summary>
		public bool canFire
		{
			get
			{
				foreach (var blocker in blockers)
				{
					if (!blocker.canFire)
						return false;
				}

				return canFireByTime;
			}
		}

		/// <summary>
		/// Indicates whether this weapon can fire, but has no ammo
		/// </summary>
		public bool canDryFire
		{
			get
			{
				if (ammo == null || !ammo.canDryFire)
					return false;

				return canFireByTime;
			}
		}

		/// <summary>
		/// Indicates whether this weapon has waited between shots
		/// </summary>
		private bool canFireByTime
		{
			get
			{
				// Weapon has not been fired OR fire every frame
				if (m_lastShotTime <= 0 || isContinuous)
					return true;

				if (!isBurstFire || m_burstThread != null)
				{
					return Time.time >= m_lastShotTime + timeBetweenShots;
				}

				return m_burstThread == null;
			}
		}

		/// <summary>
		/// Get list of shooters controlled by this controller
		/// </summary>
		public Shooter[] shooters
		{
			get
			{
#if !UNITY_EDITOR
				if (m_shooters == null)
				{
					m_shooters = GetComponents<Shooter>();
				}
				return m_shooters;
#else
				return GetComponents<Shooter>();
#endif
			}
		}

		/// <summary>
		/// Internal ammo
		/// </summary>
		public ShooterAmmo ammo
		{
			get
			{
				if (m_ammo == null)
				{
					m_ammo = GetComponent<ShooterAmmo>();
				}
				return m_ammo;
			}
		}

		/// <summary>
		/// Get list of blockers that can prevent this controller from firing
		/// </summary>
		private ShooterBlocker[] blockers
		{
			get
			{
				if (m_blockers == null)
				{
					m_blockers = GetComponents<ShooterBlocker>();
				}
				return m_blockers;
			}
		}

		/// <summary>
		/// Number of shots per second
		/// </summary>
		public float shotsPerSecond
		{
			get
			{
				if (isContinuous)
				{
					return 1f;
				}
				else
				{
					float totalTime = 0, totalShots = 1;
					if (ammo == null)
					{
						if (!isBurstFire)
						{
							totalTime += timeBetweenShots;
						}
						else
						{
							totalShots = shotsPerBurst;
							totalTime += timeBetweenBursts
								+ ((shotsPerBurst - 1f) * timeBetweenShots);
						}
					}
					else
					{
						totalTime = ammo.reloadTime;
						totalShots = ammo.shotsPerMagazine;

						if (!ammo.isSimultaneousReload)
						{
							totalTime += ammo.consecutiveReloadTime * (ammo.shotsPerMagazine - 1);
						}

						if (!isBurstFire)
						{
							totalTime += (totalShots - 1f) * timeBetweenShots;
						}
						else
						{
							totalTime += (ammo.burstsPerMagazine - 1f) * timeBetweenBursts
								+ ((shotsPerBurst - 1f) * timeBetweenShots) * ammo.burstsPerMagazine;
						}
					}

					return totalShots / totalTime;
				}
			}
		}

		/// <summary>
		/// Maximum potential damage per second
		/// </summary>
		public float damagePerSecond
		{
			get
			{
				float damagePerSecond = 0f;
				if (shooters != null)
				{
					damagePerSecond = shooters.Sum(x => x.totalDamage);
					if (!isContinuous)
					{
						damagePerSecond *= shotsPerSecond;
					}
				}

				return damagePerSecond;
			}
		}

		#endregion

		#region Methods

		protected virtual void Update()
		{
			if (!isPlayerControlled)
				return;

			if (Input.GetButtonDown(fireButton))
			{
				onBeginFire.Invoke();

				if (fireOnButtonDown && !isAutoFire)
				{
					AttemptFire();
				}
			}
			else if (Input.GetButton(fireButton))
			{
				if (fireOnButtonDown && isAutoFire)
				{
					AttemptFire();
				}
			}
			else if (Input.GetButtonUp(fireButton))
			{
				if (!fireOnButtonDown)
				{
					AttemptFire();
				}

				onEndFire.Invoke();
			}
		}

		public void AttemptFire()
		{
			if (canFire)
			{
				onShotFiring.Invoke();

				foreach (var shooter in shooters)
				{
					if (shooter == null)
						continue;

					shooter.Fire();
				}

				onShotFired.Invoke();
				Fire();
			}
			else if (canDryFire)
			{
				onDryFired.Invoke();
				Fire();
			}
			else if (isContinuous)
			{
				EndFire();
			}
		}

		private void Fire()
		{
			m_lastShotTime = Time.time;

			if (!isContinuous && isBurstFire)
			{
				++m_burstShotCount;

				if (m_burstThread == null)
				{
					m_burstThread = StartCoroutine(BeginBurstFire());
				}
			}
		}

		private void BeginFire()
		{
			if (m_isFiring)
				return;

			m_isFiring = true;
			onBeginFire.Invoke();
		}

		private IEnumerator BeginBurstFire()
		{
			while (m_burstShotCount < shotsPerBurst)
			{
				yield return new WaitForSeconds(timeBetweenShots);
				AttemptFire();
			}

			if (ammo == null || ammo.canFire)
			{
				yield return new WaitForSeconds(timeBetweenBursts);
			}

			// Reset for next burst
			m_burstShotCount = 0;
			m_burstThread = null;
		}

		private void EndFire()
		{
			if (!m_isFiring)
				return;

			m_isFiring = false;
			onEndFire.Invoke();
		}

#endregion
	}
}