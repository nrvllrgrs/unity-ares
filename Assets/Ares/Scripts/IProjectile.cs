using UnityEngine;
using Ares.Data;
using Ares.Events;

namespace Ares
{
	public interface IProjectile
	{
		#region Variables

		ProjectileData data { get; }
		Collider collider { get; }
		CollisionCallback collisionCallback { get; }
		Rigidbody rigidbody { get; }

		// Include implicit MonoBehaviour properties
		Transform transform { get; }
		GameObject gameObject { get; }

		#endregion
	}
}