using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public class ProjectileShooterData : ShooterData
	{
		#region Variables

		public IProjectile projectileTemplate;

		// Controls
		public bool spawnOnBeginFire;

		// Tracking
		public bool canTrack;
		public float maxLockTime;

		private IProjectile m_pendingProjectile;

		#endregion

		#region Properties

		public override DamageInfo damageInfo
		{
			get
			{
				if (projectileTemplate == null)
					return null;

				return projectileTemplate.data.damage;
			}
		}

		public override float totalDamage
		{
			get
			{
				if (damageInfo == null)
					return 0f;

				return damageInfo.impactDamage + damageInfo.splashDamage;
			}
		}

		#endregion

		#region Constructors

		public ProjectileShooterData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public override void Initialize()
		{
			if (spawnOnBeginFire)
			{
				controller.data.onBeginFire.AddListener(() =>
				{
					SpawnProjectile(true);
				});
			}
		}

		public override void Fire()
		{
			if (!spawnOnBeginFire)
			{
				SpawnProjectile(false);
			}
			else
			{
				// Detach projectile from muzzleBone
				m_pendingProjectile.rigidbody.isKinematic = false;
				m_pendingProjectile.transform.SetParent(null);
			}

			FireProjectile();
		}

		protected virtual void SpawnProjectile(bool attach)
		{
			var clone = Object.Instantiate(projectileTemplate.gameObject, muzzleBone.position, muzzleBone.rotation);
			m_pendingProjectile = clone.GetComponent<IProjectile>();

			var collider = owner.GetComponent<Collider>();
			if (collider != null)
			{
				Physics.IgnoreCollision(collider, m_pendingProjectile.collider);
			}

			//if (m_pendingProjectile.faction != null && controller.carrier != null)
			//{
			//	Faction carrierFaction = controller.carrier.GetComponent<Faction>();
			//	if (carrierFaction != null)
			//	{
			//		m_pendingProjectile.faction.id = carrierFaction.id;
			//	}
			//}

			if (attach)
			{
				m_pendingProjectile.rigidbody.isKinematic = true;
				m_pendingProjectile.transform.SetParent(muzzleBone);
			}
		}

		protected virtual void FireProjectile()
		{
			if (m_pendingProjectile == null)
				return;

			m_pendingProjectile.data.Fire();
			m_pendingProjectile = null;
		}

		#endregion
	}
}
