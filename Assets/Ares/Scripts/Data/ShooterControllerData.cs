using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Data
{
	[System.Serializable]
	public class ShooterControllerData : AresData
	{
		#region Variables

		/// <summary>
		/// Indicates whether player controls this controller
		/// </summary>
		[Tooltip("Indicates whether player controls this controller")]
		public bool isPlayerControlled;

		/// <summary>
		/// Indicates whether "Fire" action triggers every frame
		/// </summary>
		[Tooltip("Indicates whether \"Fire\" action triggers every frame")]
		public bool isContinuous = false;

		/// <summary>
		/// Indicates whether "Fire" action automatically triggers on button down
		/// </summary>
		[Tooltip("Indicates whether \"Fire\" action automatically triggers on button down")]
		public bool isAutoFire = false;

		/// <summary>
		/// Seconds between shots
		/// </summary>
		[Tooltip("Seconds between shots")]
		public float timeBetweenShots;

		/// <summary>
		/// Indicates whether "Fire" action occurs on button down
		/// </summary>
		[Tooltip("Indicates whether \"Fire\" action occurs on button down")]
		public bool fireOnButtonDown = true;

		/// <summary>
		/// Name of Fire button
		/// </summary>
		[Tooltip("Name of Fire button")]
		public string fireButton = "Fire1";

		/// <summary>
		/// Indicates whether "Fire" action triggers multiple shots
		/// </summary>
		[Tooltip("Indicates whether \"Fire\" action triggers multiple shots")]
		public bool isBurstFire = false;

		/// <summary>
		/// Seconds between bursts
		/// </summary>
		[Tooltip("Seconds between bursts")]
		public float timeBetweenBursts;

		/// <summary>
		/// Number of shots per burst
		/// </summary>
		[Tooltip("Number of shots per burst")]
		public int shotsPerBurst;

		private bool m_isFiring;
		private float m_beginFireTime, m_lastShotTime;
		private int m_burstShotCount;
		private Coroutine m_burstThread;

		private GameObject m_carrier;
		private IShooterAmmo m_ammo;
		private IShooterCharge m_charge;
		private IShooterBlocker[] m_blockers;
		private IShooter[] m_shooters;

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
					if (!blocker.blockerData.canFire)
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
				if (ammo == null || !ammo.data.canDryFire)
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
		public IShooter[] shooters
		{
			get
			{
				if (owner == null)
					return null;

#if !UNITY_EDITOR
				if (m_shooters == null)
				{
					m_shooters = controller.GetComponents<Shooter>();
				}
				return m_shooters;
#else
				return owner.GetComponents<IShooter>();
#endif
			}
		}

		/// <summary>
		/// Internal ammo
		/// </summary>
		public IShooterAmmo ammo
		{
			get
			{
				if (owner == null)
					return null;

#if !UNITY_EDITOR
				if (m_ammo == null)
				{
					m_ammo = owner.GetComponent<IShooterAmmo>();
				}
				return m_ammo;
#else
				return owner.GetComponent<IShooterAmmo>();
#endif
			}
		}

		public IShooterCharge charge
		{
			get
			{
				if (owner == null)
					return null;

#if !UNITY_EDITOR
				if (m_charge == null)
				{
					m_charge = owner.GetComponent<IShooterCharge>();
				}
				return m_charge;
#else
				return owner.GetComponent<IShooterCharge>();
#endif
			}
		}

		/// <summary>
		/// Get list of blockers that can prevent this controller from firing
		/// </summary>
		private IShooterBlocker[] blockers
		{
			get
			{
				if (m_blockers == null)
				{
					m_blockers = owner.GetComponents<IShooterBlocker>();
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
							totalTime += timeBetweenBursts + ((shotsPerBurst - 1f) * timeBetweenShots);
						}
					}
					else
					{
						if (ammo.data.useMagazine)
						{
							totalTime = ammo.data.reloadTime;
							totalShots = ammo.data.shotsPerMagazine;

							if (isBurstFire)
							{
								totalTime += (ammo.data.burstsPerMagazine - 1f) * timeBetweenBursts
									+ ((shotsPerBurst - 1f) * timeBetweenShots) * ammo.data.burstsPerMagazine;
							}
							else
							{
								totalTime += (ammo.data.shotsPerMagazine - 1) * timeBetweenShots;
							}

							if (!ammo.data.isSimultaneousReload)
							{
								totalTime += ammo.data.consecutiveReloadTime * (ammo.data.shotsPerMagazine - 1);
							}
						}
						else if (isBurstFire)
						{
							totalShots = shotsPerBurst;
							totalTime += timeBetweenBursts + ((shotsPerBurst - 1f) * timeBetweenShots);
						}
						else if (!ammo.data.isInfinite)
						{
							totalShots = ammo.data.capacity;
							totalTime = (totalShots - 1) * timeBetweenShots;
						}
						else
						{
							totalTime = timeBetweenShots;
						}
					}

					if (charge != null)
					{
						totalTime += charge.data.maxCharge;
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
					damagePerSecond = shooters.Sum(x => x.shooterData.totalDamage);
					if (!isContinuous)
					{
						damagePerSecond *= shotsPerSecond;
					}
				}

				return damagePerSecond;
			}
		}

		#endregion

		#region Constructors

		public ShooterControllerData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void Tick()
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

					shooter.shooterData.Fire();
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
					m_burstThread = owner.StartCoroutine(BeginBurstFire());
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

			if (ammo == null || ammo.data.canFire)
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
