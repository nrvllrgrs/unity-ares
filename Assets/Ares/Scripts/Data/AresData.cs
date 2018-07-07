using UnityEngine;

namespace Ares.Data
{
	public abstract class AresData
	{
		#region Variables

		[SerializeField]
		private MonoBehaviour m_owner;

		#endregion

		#region Properties

		/// <summary>
		/// MonoBehaviour that owns this data
		/// </summary>
		public MonoBehaviour owner
		{
			get { return m_owner; }
			set { m_owner = value; }
		}

		#endregion

		#region Constructors

		protected AresData(MonoBehaviour owner)
		{
			if (owner == null)
			{
				throw new System.ArgumentNullException("owner");
			}

			this.owner = owner;
		}

		#endregion

		#region Methods

		public virtual void Initialize()
		{ }

		#endregion
	}
}
