using UnityEngine;

namespace Ares
{
	[System.Serializable]
	public class DamageInfo : ICopyable<DamageInfo>
	{
		#region Variables

		[Tooltip("Amount of damage dealt upon impact")]
		public float impactDamage;

		// TODO: Damage type

		public float impactImpulse;
		public float impactRange;
		public AnimationCurve impactFalloff;

		public float splashDamage;

		// TODO: Damage type

		public float splashImpulse;
		public float splashRange;
		public AnimationCurve splashFalloff;

		public LayerMask layerMask;

		#endregion

		#region Properties

		public float range
		{
			get
			{
				return impactRange == 0
					? System.Single.MaxValue
					: impactRange;
			}
		}

		#endregion

		#region Methods

		public void CopyTo(DamageInfo other)
		{
			other.impactDamage = impactDamage;
			other.impactImpulse = impactImpulse;
			other.impactRange = impactRange;
			other.impactFalloff = impactFalloff;

			other.splashDamage = splashDamage;
			other.splashImpulse = splashImpulse;
			other.splashRange = splashRange;
			other.splashFalloff = splashFalloff;
		}

		public void ApplyDamage(GameObject victim, Vector3 origin, Vector3 contact, Vector3 normal)
		{
			ApplyDamage(victim, origin, contact, normal, 1f, 1f);
		}

		public void ApplyDamage(GameObject victim, Vector3 origin, Vector3 contact, Vector3 normal, float impactDamageFactor, float splashDamageFactor)
		{
			float distance = 1f;
			if (impactRange > 0f && impactFalloff != null)
			{
				distance = Vector3.Distance(origin, victim.transform.position);
			}

			ApplyDamage(victim, distance, contact, normal, impactDamageFactor, splashDamageFactor);
		}

		public void ApplyDamage(GameObject victim, float distance, Vector3 contact, Vector3 normal)
		{
			ApplyDamage(victim, distance, contact, normal, 1f, 1f);
		}

		public void ApplyDamage(GameObject victim, float distance, Vector3 contact, Vector3 normal, float impactDamageFactor, float splashDamageFactor)
		{
			ApplyImpactDamage(victim, distance, contact, normal, impactDamageFactor);
			ApplySplashDamage(contact, splashDamageFactor);
		}

		private void ApplyImpactDamage(GameObject victim, float distance, Vector3 contact, Vector3 normal, float damageFactor)
		{
			if (victim == null)
				return;

			var health = victim.GetComponent<IHealth>();
			if (health != null)
			{
				float f = 1f;
				if (impactRange > 0f && impactFalloff != null)
				{
					f = impactFalloff.Evaluate(distance / impactRange);
				}

				health.data.Damage(new DamageActionInfo(impactDamage * f * damageFactor));
			}

			var rigidbody = victim.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				var force = -normal * impactImpulse;
				rigidbody.AddForceAtPosition(force, contact, ForceMode.Impulse);
			}
		}

		private void ApplySplashDamage(Vector3 contact, float damageFactor)
		{
			if (splashDamage == 0 || splashRange <= 0)
				return;

			foreach (var collider in Physics.OverlapSphere(contact, splashRange))
			{
				var health = collider.GetComponent<IHealth>();
				if (health != null)
				{
					float f = 1f;
					if (splashFalloff != null)
					{
						f = splashFalloff.Evaluate(Vector3.Distance(contact, health.transform.position) / splashRange);
					}

					health.data.Damage(new DamageActionInfo(splashDamage * f * damageFactor));
				}

				var rigidbody = collider.GetComponent<Rigidbody>();
				if (rigidbody != null)
				{
					rigidbody.AddExplosionForce(splashImpulse, contact, splashRange);
				}
			}
		}

		#endregion
	}

	public class DamageActionInfo
	{
		#region Properties

		public float value { get; private set; }
		public int killerId { get; private set; }
		public int victimId { get; private set; }

		#endregion

		#region Constructors

		public DamageActionInfo(float value)
			: this(value, -1, -1)
		{ }

		public DamageActionInfo(float value, int killerId, int victimId)
		{
			this.value = value;
			this.killerId = killerId;
			this.victimId = victimId;
		}

		#endregion
	}

	public interface IDamage
	{
		#region Properties

		DamageInfo damageInfo { get; }

		#endregion
	}
}
