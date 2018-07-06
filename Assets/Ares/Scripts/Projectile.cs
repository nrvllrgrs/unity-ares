using UnityEngine;
using Ares.Data;
using Ares.Events;

namespace Ares
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(CollisionCallback))]
	[RequireComponent(typeof(Rigidbody))]
	public class Projectile : AresMonoBehaviour<ProjectileData>, IProjectile
	{
		#region Variables

		private Collider m_collider;
		private CollisionCallback m_collisionCallback;
		private Rigidbody m_rigidbody;

		#endregion

		#region Properties

		public new Collider collider
		{
			get
			{
				if (m_collider == null)
				{
					m_collider = GetComponent<Collider>();
				}
				return m_collider;
			}
		}

		public CollisionCallback collisionCallback
		{
			get
			{
				if (m_collisionCallback == null)
				{
					m_collisionCallback = GetComponent<CollisionCallback>();
				}
				return m_collisionCallback;
			}
		}

		public new Rigidbody rigidbody
		{
			get
			{
				if (m_rigidbody == null)
				{
					m_rigidbody = GetComponent<Rigidbody>();
				}
				return m_rigidbody;
			}
		}

		#endregion

		#region Methods

		protected override void Reset()
		{
			data = new ProjectileData(this);
		}

		public void Detonate()
		{
			data.Detonate();
		}

		#endregion
	}
}
