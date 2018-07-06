using UnityEngine;
using UnityEngine.Events;

namespace Ares.Events
{
	[RequireComponent(typeof(Collider))]
	public class MouseCallback : MonoBehaviour
	{
		#region Events
		
		public UnityEvent onMouseDown = new UnityEvent();
		public UnityEvent onMouseDrag = new UnityEvent();
		public UnityEvent onMouseEnter = new UnityEvent();
		public UnityEvent onMouseExit = new UnityEvent();
		public UnityEvent onMouseOver = new UnityEvent();
		public UnityEvent onMouseUp = new UnityEvent();

		#endregion

		#region Methods

		private void OnMouseDown()
		{
			onMouseDown.Invoke();
		}

		private void OnMouseDrag()
		{
			onMouseDrag.Invoke();
		}

		private void OnMouseEnter()
		{
			onMouseEnter.Invoke();
		}

		private void OnMouseExit()
		{
			onMouseExit.Invoke();
		}

		private void OnMouseOver()
		{
			onMouseOver.Invoke();
		}

		private void OnMouseUp()
		{
			onMouseUp.Invoke();
		}

		#endregion
	}
}
