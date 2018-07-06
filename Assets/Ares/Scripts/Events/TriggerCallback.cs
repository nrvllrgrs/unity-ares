using UnityEngine;
using UnityEngine.Events;

namespace Ares.Events
{
	[System.Serializable]
	public class TriggerEvent : UnityEvent<Collider>
	{ }

	[RequireComponent(typeof(Collider))]
	public class TriggerCallback : MonoBehaviour
	{
		#region Events

		public TriggerEvent onTriggerEnter = new TriggerEvent();
		public TriggerEvent onTriggerStay = new TriggerEvent();
		public TriggerEvent onTriggerExit = new TriggerEvent();

		#endregion

		#region Methods

		private void OnTriggerEnter(Collider other)
		{
			onTriggerEnter.Invoke(other);
		}

		private void OnTriggerStay(Collider other)
		{
			onTriggerStay.Invoke(other);
		}

		private void OnTriggerExit(Collider other)
		{
			onTriggerExit.Invoke(other);
		}

		#endregion
	}
}
