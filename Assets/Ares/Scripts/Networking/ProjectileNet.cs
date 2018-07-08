using UnityEngine;
using UnityEngine.Networking;
using Ares.Data;
using Ares.Events;

namespace Ares.Networking
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(CollisionCallback))]
	[RequireComponent(typeof(Rigidbody))]
	public class ProjectileNet : NetworkBehaviour, IProjectile
	{
		#region Variables

		private Collider m_collider;
		private CollisionCallback m_collisionCallback;
		private Rigidbody m_rigidbody;

		[SerializeField]
		private ProjectileData m_data;

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

		public ProjectileData data
		{
			get { return m_data; }
			private set { m_data = value; }
		}

		#endregion

		#region Methods

		private void Reset()
		{
			data = new ProjectileData(this);
		}

		public override void OnStartAuthority()
		{
			data.Initialize();
		}

		#endregion

		#region Commands



		#endregion

		#region Rpc



		#endregion
	}
}
