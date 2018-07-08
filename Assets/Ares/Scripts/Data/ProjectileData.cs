using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public class ProjectileData : AresData, ICopyable<ProjectileData>
	{
		#region Variables

		[SerializeField]
		private IProjectile m_projectile;

		public DamageInfo damage;

		//public CollisionDetonationType collisionDetonationType;

		// Physics
		public float speed;
		public float acceleration;
		public float maxSpeed;

		// Tracking
		public GameObject target;
		public float angularSpeed;

		private Vector3 m_prevPosition;
		private float m_distance;

		#endregion

		#region Properties

		public IProjectile projectile
		{
			get
			{
				if (m_projectile == null)
				{
					m_projectile = (IProjectile)owner;
				}
				return m_projectile;
			}
			set { m_projectile = value; }
		}

		#endregion

		#region Constructors

		public ProjectileData(MonoBehaviour owner)
			: base(owner)
		{
			projectile = owner as IProjectile;
		}

		#endregion

		#region Methods

		public void CopyTo(ProjectileData other)
		{
			if (other.damage == null)
			{
				other.damage = new DamageInfo();
			}

			damage.CopyTo(other.damage);

			other.speed = speed;
			other.acceleration = acceleration;
			other.maxSpeed = maxSpeed;
			other.target = target;
			other.angularSpeed = angularSpeed;
		}

		public override void Initialize()
		{
			m_prevPosition = owner.transform.position;

			if (projectile == null)
			{
				Debug.LogError(string.Format("Projectile is undefined for {0}!", owner.name));
				return;
			}
			projectile.collisionCallback.onCollisionEnter.AddListener(OnImpact);
		}

		public void Tick()
		{
			// Ignore if range is infinite
			if (damage.impactRange > 0)
			{
				// TODO: Extend Vector3 to include SqrDistance
				m_distance += Vector3.Distance(m_prevPosition, owner.transform.position);

				if (m_distance >= damage.impactRange)
				{
					damage.ApplyDamage(null, damage.impactRange, owner.transform.position, owner.transform.position - m_prevPosition);

					// Destroy self
					Object.Destroy(owner.gameObject);
				}

				m_prevPosition = owner.transform.position;
			}

			// Accelerate, if necessary
			if (acceleration > 0 && m_projectile.rigidbody.velocity.magnitude < maxSpeed)
			{
				m_projectile.rigidbody.velocity += owner.transform.forward * acceleration * Time.deltaTime;
			}

			if (target != null)
			{
				float step = angularSpeed * Mathf.Deg2Rad * Time.deltaTime;
				var direction = Vector3.RotateTowards(owner.transform.forward, target.transform.position, step, 0f);
				owner.transform.rotation = Quaternion.LookRotation(direction);
			}
		}

		private void OnImpact(Collision collision)
		{
			//if (collisionDetonationType == CollisionDetonationType.None)
			//	return;

			//if (collisionDetonationType == CollisionDetonationType.Enemy)
			//{
			//	if (faction == null)
			//	{
			//		Debug.LogError(string.Format("Faction component not found for projectile {0}! Cannot detonate on enemy!", name));
			//		return;
			//	}

			//	// Ignore collisions of same faction
			//	Faction collisionFaction = collision.gameObject.GetComponent<Faction>();
			//	if (collisionFaction == null || collisionFaction.id == faction.id)
			//		return;
			//}

			ContactPoint contactPoint = collision.contacts[0];
			damage.ApplyDamage(collision.gameObject, m_distance, contactPoint.point, contactPoint.normal);

			// Destroy self
			Object.Destroy(owner.gameObject);
		}

		public void Fire()
		{
			// TODO: Scale speed by transform.scale
			if (projectile != null)
			{
				projectile.rigidbody.velocity += owner.transform.forward * speed;
			}
		}

		public virtual void Detonate()
		{
			damage.ApplyDamage(null, damage.impactRange, owner.transform.position, owner.transform.up);
			Object.Destroy(owner.gameObject);
		}

		#endregion
	}
}