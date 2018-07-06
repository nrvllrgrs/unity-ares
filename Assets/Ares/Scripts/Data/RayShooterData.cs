using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public class RayShooterData : ShooterData
	{
		#region Variables

		public DamageInfo damage;

		// TODO: Tracer

#if UNITY_EDITOR

		// Debug
		public bool drawRay;

#endif
		#endregion

		#region Properties

		public override DamageInfo damageInfo
		{
			get { return damage; }
		}

		#endregion

		#region Constructors

		public RayShooterData(MonoBehaviour owner)
			: base(owner)
		{
			damage = new DamageInfo();
		}

		#endregion

		#region Methods

		public override void Fire()
		{
			RaycastHit hit;
			bool hitResult = Physics.Raycast(muzzleBone.position, muzzleBone.forward, out hit, damage.range, damage.layerMask);

#if UNITY_EDITOR
			DrawRay(hitResult, hit, muzzleBone.forward);
#endif

			Hit(hitResult, hit);
		}

		protected virtual void Hit(bool hitResult, RaycastHit hit)
		{
			if (hitResult)
			{
				damage.ApplyDamage(hit.collider.gameObject, muzzleBone.position, hit.point, hit.normal);
			}
		}

		#endregion

		#region Editor Methods
#if UNITY_EDITOR

		protected void DrawRay(bool hitResult, RaycastHit hit, Vector3 direction)
		{
			if (drawRay)
			{
				if (hitResult)
				{
					Debug.DrawLine(muzzleBone.position, hit.point, Color.red, 1f, false);
				}
				else
				{
					Debug.DrawRay(muzzleBone.position, direction * damage.impactRange, Color.blue, 1f, false);
				}
			}
		}

#endif
		#endregion
	}
}