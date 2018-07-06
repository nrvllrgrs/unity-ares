using UnityEngine;
using UnityEngine.Events;

namespace Ares.Events
{
	[System.Serializable]
	public class CollisionEvent : UnityEvent<Collision>
	{ }

	[RequireComponent(typeof(Collider))]
	public class CollisionCallback : MonoBehaviour
	{
		#region Events

		public CollisionEvent onCollisionEnter = new CollisionEvent();
		public CollisionEvent onCollisionStay = new CollisionEvent();
		public CollisionEvent onCollisionExit = new CollisionEvent();

		#endregion

		#region Methods

		private void OnCollisionEnter(Collision collision)
		{
			onCollisionEnter.Invoke(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			onCollisionStay.Invoke(collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			onCollisionExit.Invoke(collision);
		}

		#endregion
	}
}
