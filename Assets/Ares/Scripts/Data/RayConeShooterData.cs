using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public class RayConeShooterData : RayShooterData, ICopyable<RayShooterData>
	{
		#region Variables

		/// <summary>
		/// Number of rays
		/// </summary>
		[Tooltip("Number of rays")]
		public int count;

		/// <summary>
		/// Maximum angle rays from forward (in degrees)
		/// </summary>
		[Tooltip("Maximum angle rays from forward (in degrees)")]
		public float maxAngle;

		/// <summary>
		/// Indicates whether rays use normal distribution
		/// </summary>
		[Tooltip("Indicates whether rays use normal distribution")]
		public bool isNormalDistribution;

		#endregion

		#region Properties

		public override float totalDamage
		{
			get { return base.totalDamage * count; }
		}

		#endregion

		#region Constructors

		public RayConeShooterData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void CopyTo(RayConeShooterData other)
		{
			base.CopyTo(other);

			other.count = count;
			other.maxAngle = maxAngle;
			other.isNormalDistribution = isNormalDistribution;
		}

		public override void Fire()
		{
			if (!isNormalDistribution)
			{
				for (int i = 0; i < count; ++i)
				{
					// Need to use negative maxAngle to mirror Gaussian randomness
					FireRay(Random.Range(-180f, 180f), Random.Range(-maxAngle, maxAngle));
				}
			}
			else
			{
				for (int i = 0; i < count; ++i)
				{
					FireRay(Random.Range(-180f, 180f), Mathf.Clamp(AresUtility.NextGaussian(0f, 0.2f), -1f, 1f) * maxAngle);
				}
			}
		}

		private void FireRay(float angle, float radius)
		{
			var direction = Quaternion.AngleAxis(angle, muzzleBone.forward)
				* Quaternion.AngleAxis(radius, muzzleBone.right)
				* muzzleBone.forward;

			RaycastHit hit;
			bool hitResult = Physics.Raycast(muzzleBone.position, direction.normalized, out hit, damage.range, damage.layerMask);

#if UNITY_EDITOR
			DrawRay(hitResult, hit, direction);
#endif

			Hit(hitResult, hit);
		}

		#endregion
	}
}