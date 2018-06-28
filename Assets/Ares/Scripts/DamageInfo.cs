﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ares
{
	[System.Serializable]
	public class DamageInfo
	{
		#region Variables

		public float impactDamage;

		// Damage type

		public float impactImpulse;
		public float impactRange;
		public AnimationCurve impactFalloff;

		public float splashDamage;

		// Damage type

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

			var health = victim.GetComponent<Health>();
			if (health != null)
			{
				float f = 1f;
				if (impactRange > 0f && impactFalloff != null)
				{
					f = impactFalloff.Evaluate(distance / impactRange);
				}

				//!!!health.Damage(new DamageActionInfo(impactDamage * f * damageFactor));
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
				var health = collider.GetComponent<Health>();
				if (health != null)
				{
					float f = 1f;
					if (splashFalloff != null)
					{
						f = splashFalloff.Evaluate(Vector3.Distance(contact, health.transform.position) / splashRange);
					}

					//!!!health.Damage(new DamageActionInfo(splashDamage * f * damageFactor));
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

	public interface IDamage
	{
		#region Properties

		DamageInfo damageInfo { get; }

		#endregion
	}
}