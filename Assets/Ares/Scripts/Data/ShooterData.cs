﻿using UnityEngine;

namespace Ares.Data
{
	[System.Serializable]
	public abstract class ShooterData : AresData, ICopyable<ShooterData>
	{
		#region Variables

		/// <summary>
		/// Position and rotation of shot origin
		/// </summary>
		[Tooltip("Position and rotation of shot origin")]
		public Transform muzzleBone;

		/// <summary>
		/// Shot-fired audio clip
		/// </summary>
		[Tooltip("Shot-fired audio clip")]
		public AudioClip clip;

		/// <summary>
		/// Shot-fired audio clip volume
		/// </summary>
		[Tooltip("Shot-fired audio clip volume")]
		public float volume = 1f;

		// TODO: Do NOT spawn raw VFX
		// Need to implement surface-surface interaction solution (see Hecate of asset store)
		// Then ray/projectile/etc is assigned a surface type

		private IShooterController m_controller;

		#endregion

		#region Properties

		public IShooterController controller
		{
			get
			{
				if (m_controller == null)
				{
					m_controller = owner.GetComponent<IShooterController>();
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

		#region Constructors

		public ShooterData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion

		#region Methods

		public void CopyTo(ShooterData other)
		{
			other.muzzleBone = muzzleBone;
			other.clip = clip;
			other.volume = volume;
		}

		public override void Initialize()
		{
			if (muzzleBone == null)
			{
				muzzleBone = owner.transform;
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
