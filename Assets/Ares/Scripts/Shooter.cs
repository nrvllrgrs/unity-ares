using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ares
{
	[RequireComponent(typeof(ShooterController))]
	public abstract class Shooter : MonoBehaviour, IDamage
	{
		#region Variables

		public Transform muzzleBone;

		// Audio
		public AudioClip clip;
		public float volume = 1f;

		// TODO: Do NOT spawn raw VFX
		// Need to implement surface-surface interaction solution (see Hecate of asset store)
		// Then ray/projectile/etc is assigned a surface type

		private ShooterController m_controller;

		#endregion

		#region Properties

		public ShooterController controller
		{
			get
			{
				if (m_controller == null)
				{
					m_controller = GetComponent<ShooterController>();
				}
				return m_controller;
			}
		}

		public virtual float totalDamage
		{
			get
			{
				if (damageInfo == null)
					return 0f;

				return damageInfo.impactDamage + damageInfo.splashDamage;
			}
		}

		public abstract DamageInfo damageInfo { get; }

		#endregion

		#region Methods

		protected virtual void Awake()
		{
			if (muzzleBone == null)
			{
				muzzleBone = transform;
			}

			if (clip != null)
			{
				if (controller.data.isContinuous)
				{
					var audio = muzzleBone.GetComponent<AudioSource>();
					if (audio == null)
					{
						audio = muzzleBone.gameObject.AddComponent<AudioSource>();
						audio.loop = true;
						audio.playOnAwake = false;
						audio.spatialBlend = 1f;
					}

					audio.clip = clip;
					audio.volume = volume;

					controller.data.onBeginFire.AddListener(() =>
					{
						audio.Play();
					});

					controller.data.onEndFire.AddListener(() =>
					{
						audio.Stop();
					});
				}
				else
				{
					controller.data.onShotFired.AddListener(() =>
					{
						AudioSource.PlayClipAtPoint(clip, muzzleBone.position, volume);
					});
				}
			}
		}

		public abstract void Fire();

		#endregion
	}
}
