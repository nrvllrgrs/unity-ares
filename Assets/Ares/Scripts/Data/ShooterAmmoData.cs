using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Data
{
	[System.Serializable]
	public class ShooterAmmoData : ShooterBlockerData
	{
		#region Variables

		/// <summary>
		/// Indicates whether there is no upper-bound to the number of carried shots
		/// </summary>
		[Tooltip("Indicates whether there is no upper-bound to the number of carried shots")]
		public bool isInfinite;

		/// <summary>
		/// Maximum number of shots that can be carried
		/// </summary>
		[Tooltip("Maximum number of shots that can be carried")]
		public int capacity;

		/// <summary>
		/// Number of shots currently carried
		/// </summary>
		[SerializeField]
		[Tooltip("Number of shots currently carried")]
		private int m_count;

		/// <summary>
		/// Indicates whether weapon uses a magazine
		/// </summary>
		[Tooltip("Indicates whether weapon uses a magazine")]
		public bool useMagazine;

		/// <summary>
		/// Maximum number of magazines that can be carried
		/// </summary>
		[Tooltip("Maximum number of magazines that can be carried")]
		public int magazineCapacity;

		/// <summary>
		/// Number of magazines currently carried
		/// </summary>
		[Tooltip("Number of magazines currently carried")]
		public int magazineCount;

		/// <summary>
		/// Number of shots per magazine
		/// </summary>
		[Tooltip("Number of shots per magazine")]
		public int shotsPerMagazine;

		/// <summary>
		/// Number of shots currently in magazine
		/// </summary>
		[SerializeField]
		[Tooltip("Number of shots currently in magazine")]
		private int m_shotsInMagazine;

		/// <summary>
		/// Indicates whether shooter is automatically reloaded
		/// </summary>
		[Tooltip("Indicates whether shooter is automatically reloaded")]
		public bool isAutoReload;

		/// <summary>
		/// Indicates whether all shots in magazine are reloaded simultaneously
		/// </summary>
		[Tooltip("Indicates whether all shots in magazine are reloaded simultaneously")]
		public bool isSimultaneousReload = true;

		/// <summary>
		/// Seconds to reload
		/// </summary>
		[Tooltip("Seconds to reload")]
		public float reloadTime;

		/// <summary>
		/// Seconds to reload
		/// </summary>
		[Tooltip("Seconds to reload")]
		public float consecutiveReloadTime;

		/// <summary>
		/// Name of Reload button
		/// </summary>
		[Tooltip("Name of Reload button")]
		public string reloadButton = "Reload";

		/// <summary>
		/// Indicates whether ammo can regenerate
		/// </summary>
		[Tooltip("Indicates whether ammo can regenerate")]
		public bool useRegeneration;

		/// <summary>
		/// Seconds to wait before regeneration begins
		/// </summary>
		[Tooltip("Seconds to wait before regeneration begins")]
		public float regenerationDelay;

		/// <summary>
		/// Number of ammo regenerated per second
		/// </summary>
		[Tooltip("Number of ammo regenerated per second")]
		public float regenerationRate;

		// TODO: Shots consumed per "Fire"
		// May want nanobot ammo similar to Invisible War

		private bool m_isReloading;
		private Coroutine m_reloadThread, m_regenerationThread;

		#endregion

		#region Events

		public UnityEvent onCountChanged = new UnityEvent();
		public UnityEvent onBeginReload = new UnityEvent();
		public UnityEvent onEndReload = new UnityEvent();

		#endregion

		#region Properties

		public override bool canFire
		{
			get
			{
				if (isReloading)
					return false;

				return useMagazine
					? magazineCount > 0
					: count > 0;
			}
		}

		public bool canDryFire
		{
			get
			{
				if (isReloading)
					return false;

				return count == 0;
			}
		}

		public bool isReloading
		{
			get { return m_isReloading; }
			private set
			{
				if (isReloading == value)
					return;

				m_isReloading = value;

				if (isReloading)
				{
					onBeginReload.Invoke();
				}
				else
				{
					onEndReload.Invoke();
				}
			}
		}

		public int count
		{
			get { return m_count; }
			set
			{
				value = Mathf.Clamp(value, 0, capacity);
				if (count == value)
					return;

				m_count = value;
				onCountChanged.Invoke();

				if (!isInfinite && useRegeneration && m_regenerationThread == null && regenerationRate > 0f)
				{
					m_regenerationThread = owner.StartCoroutine(RegenerationThread());
				}
			}
		}

		/// <summary>
		/// Gets and sets number of shots currently in magazine
		/// </summary>
		public int shotsInMagazine
		{
			get { return m_shotsInMagazine; }
			set
			{
				value = Mathf.Clamp(value, 0, shotsPerMagazine);
				if (shotsInMagazine == value)
					return;

				m_shotsInMagazine = value;
			}
		}

		/// <summary>
		/// Gets and sets number of bursts per magazine
		/// </summary>
		public int burstsPerMagazine
		{
			get
			{
				if (!controller.data.isBurstFire || controller.data.shotsPerBurst <= 0)
					return 0;

				return shotsPerMagazine / controller.data.shotsPerBurst;
			}
		}

		#endregion

		#region Constructors

		public ShooterAmmoData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public override void Initialize()
		{
			controller.data.onShotFired.AddListener(OnShotFired);

			if (!isInfinite && useMagazine)
			{
				// Removal of ammo total
				// Do not want to give extra magazine
				count -= shotsInMagazine;
			}
		}

		protected virtual void OnShotFired()
		{
			// Stop regeneration when player fires
			if (m_regenerationThread != null)
			{
				owner.StopCoroutine(m_regenerationThread);
			}

			if (useMagazine)
			{
				shotsInMagazine -= 1;

				// Auto-reload when the magazine is empty
				if (shotsInMagazine == 0 && isAutoReload)
				{
					Reload();
				}
			}
			else if (!isInfinite)
			{
				count -= 1;
			}
		}

		public void Reload()
		{
			if (m_reloadThread != null)
				return;

			m_reloadThread = owner.StartCoroutine(ReloadThread());
		}

		private IEnumerator ReloadThread()
		{
			// "Use Magazine" is implied for reloading
			// No need to check in this thread
			isReloading = true;

			if (isSimultaneousReload)
			{
				yield return new WaitForSeconds(reloadTime);

				// Add shots to magazine, but no more than currently carried
				shotsInMagazine = Mathf.Min(shotsPerMagazine, count);

				if (!isInfinite)
				{
					count -= shotsInMagazine;
				}
			}
			else
			{
				yield return new WaitForSeconds(reloadTime);

				++shotsInMagazine;

				if (!isInfinite)
				{
					--count;
				}

				while (true)
				{
					if (shotsInMagazine == shotsPerMagazine || count == 0)
						break;

					yield return new WaitForSeconds(consecutiveReloadTime);

					++shotsInMagazine;

					if (!isInfinite)
					{
						--count;
					}
				}
			}

			isReloading = false;
			m_reloadThread = null;
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
				count += 1;

				if (useMagazine)
				{
					// Carrying max capacity
					if (count == capacity - shotsPerMagazine)
					{
						break;
					}
				}
				else if (count == capacity)
				{
					break;
				}
			}

			m_regenerationThread = null;
		}

		#endregion
	}
}
