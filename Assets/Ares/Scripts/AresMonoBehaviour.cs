using UnityEngine;
using Ares.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ares
{
	public abstract class AresMonoBehaviour<T> : MonoBehaviour
		where T : AresData
	{
		#region Variables

		[SerializeField]
		private T m_data;

		#endregion

		#region Properties

		public T data
		{
			get { return m_data; }
			protected set { m_data = value; }
		}

		#endregion

		#region Methods

		protected abstract void Reset();

		protected virtual void Awake()
		{
			data.Initialize();
		}

		#endregion

		#region Editor Methods
#if UNITY_EDITOR

		protected virtual void ResetOwner()
		{
			data.owner = this;
		}

#endif
		#endregion
	}
}
