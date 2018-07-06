using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ares.Events
{
	public class WaitCallback : MonoBehaviour
	{
		#region Variables

		/// <summary>
		/// Seconds to wait before completing
		/// </summary>
		[Tooltip("Seconds to wait before completing")]
		public float time;

		/// <summary>
		/// Indicates whether timer begins immediately
		/// </summary>
		[Tooltip("Indicates whether timer begins immediately")]
		public bool runOnAwake = true;

		private Coroutine m_thread;

		#endregion

		#region Events

		public UnityEvent onCompleted = new UnityEvent();

		#endregion

		#region Methods

		private void Awake()
		{
			if (runOnAwake)
			{
				Wait();
			}
		}

		public void Wait()
		{
			if (m_thread != null)
				return;

			m_thread = StartCoroutine(WaitThread());
		}

		private IEnumerator WaitThread()
		{
			if (time > 0f)
			{
				yield return new WaitForSeconds(time);
			}
			else
			{
				yield return null;
			}

			onCompleted.Invoke();
			m_thread = null;
		}

		#endregion
	}
}