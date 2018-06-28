using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public class ShooterAmmo : ShooterBlocker
	{
		#region Variables

		/// <summary>
		/// Indicates whether there is no upper-bound to the number of carried shots
		/// </summary>
		public bool isInfinite;

		/// <summary>
		/// Maximum number of shots that can be carried
		/// </summary>
		public int capacity;

		[SerializeField]
		/// <summary>
		/// Number of shots currently carried
		/// </summary>
		private int m_count;

		/// <summary>
		/// Indicates whether weapon uses a magazine
		/// </summary>
		public bool useMagazine;

		/// <summary>
		/// Maximum number of magazines that can be carried
		/// </summary>
		public int magazineCapacity;

		/// <summary>
		/// Number of magazines currently carried
		/// </summary>
		public int magazineCount;

		/// <summary>
		/// Number of shots per magazine
		/// </summary>
		public int shotsPerMagazine;

		[SerializeField]
		/// <summary>
		/// Number of shots currently in magazine
		/// </summary>
		private int m_shotsInMagazine;

		/// <summary>
		/// Indicates whether shooter is automatically reloaded
		/// </summary>
		public bool isAutoReload;

		/// <summary>
		/// Indicates whether all shots in magazine are reloaded simultaneously
		/// </summary>
		public bool isSimultaneousReload = true;

		/// <summary>
		/// Seconds to reload
		/// </summary>
		public float reloadTime;

		/// <summary>
		/// Seconds to reload
		/// </summary>
		public float consecutiveReloadTime;

		/// <summary>
		/// Name of Reload button
		/// </summary>
		public string reloadButton = "Reload";

		/// <summary>
		/// Indicates whether ammo can regenerate
		/// </summary>
		public bool useRegeneration;

		/// <summary>
		/// Seconds to wait before regeneration begins
		/// </summary>
		public float regenerationDelay;

		/// <summary>
		/// Number of ammo regenerated per second
		/// </summary>
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
					m_regenerationThread = StartCoroutine(RegenerationThread());
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
				if (!controller.isBurstFire || controller.shotsPerBurst <= 0)
					return 0;

				return shotsPerMagazine / controller.shotsPerBurst;
			}
		}

		#endregion

		#region Methods

		protected virtual void Awake()
		{
			controller.onShotFired.AddListener(OnShotFired);

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
				StopCoroutine(m_regenerationThread);
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

			m_reloadThread = StartCoroutine(ReloadThread());
		}

		private IEnumerator ReloadThread()
		{
			// "Use Magazine" is implied for reloading
			// No need to check in this thread
			isReloading = true;

			if (isSimultaneousReload)
			{
				yield return new WaitForSeconds(reloadTime);

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
