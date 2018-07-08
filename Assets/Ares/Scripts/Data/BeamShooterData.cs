using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public class BeamShooterData : RayShooterData, ICopyable<BeamShooterData>
	{
		#region Variables

		public Beam beamTemplate;
		public GameObject endBeamTemplate;

		public bool spawnOnShotFired = true;

		private Beam m_beam;
		private GameObject m_endBeam;

		#endregion

		#region Constructors

		public BeamShooterData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void CopyTo(BeamShooterData other)
		{
			base.CopyTo(other);

			other.beamTemplate = beamTemplate;
			other.endBeamTemplate = endBeamTemplate;
			other.spawnOnShotFired = spawnOnShotFired;
		}

		public override void Initialize()
		{
			if (spawnOnShotFired)
			{
				controller.data.onShotFired.AddListener(() =>
				{
					// If both are NULL, then spawn something
					if (m_beam == null && m_endBeam == null)
					{
						SpawnBeam();
					}
				});

				controller.data.onEndFire.AddListener(() =>
				{
					DestroyBeam();
				});
			}
			else
			{
				SpawnBeam();
			}
		}

		protected virtual void SpawnBeam()
		{
			if (beamTemplate != null)
			{
				m_beam = Object.Instantiate(beamTemplate, muzzleBone.position, muzzleBone.rotation);
				m_beam.source = muzzleBone;
			}

			if (endBeamTemplate != null)
			{
				m_endBeam = Object.Instantiate(endBeamTemplate, muzzleBone.position, muzzleBone.rotation).gameObject;
				m_endBeam.transform.SetParent(muzzleBone);
			}
		}

		protected virtual void DestroyBeam()
		{
			if (m_beam != null)
			{
				Object.Destroy(m_beam.gameObject);
			}

			if (m_endBeam != null)
			{
				Object.Destroy(m_endBeam);
			}
		}

		protected override void Hit(bool hitResult, RaycastHit hit)
		{
			if (hitResult)
			{
				damage.ApplyDamage(hit.collider.gameObject, muzzleBone.position, hit.point, hit.normal, Time.deltaTime, Time.deltaTime);
			}

			if (m_beam != null)
			{
				if (hitResult)
				{
					m_beam.Refresh(muzzleBone.position, hit.point);
				}
				else
				{
					m_beam.Refresh(muzzleBone.position, muzzleBone.position + (muzzleBone.forward * (damage.impactRange > 0 ? damage.impactRange : System.Single.MaxValue)));
				}
			}

			if (m_endBeam != null)
			{
				m_endBeam.transform.position = hit.transform.position;
			}
		}

		#endregion
	}
}
