using UnityEngine;
using Ares.Data;

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
	}
}
