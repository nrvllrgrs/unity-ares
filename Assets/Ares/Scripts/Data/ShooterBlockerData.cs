using UnityEngine;

namespace Ares.Data
{
	public abstract class ShooterBlockerData : AresData
	{
		#region Variables

		private IShooterController m_controller;

		#endregion

		#region Properties

		/// <summary>
		/// Gets blocker controller
		/// </summary>
		public IShooterController controller
		{
			get
			{
				if (owner == null)
					return null;

				if (m_controller == null)
				{
					m_controller = owner.GetComponent<IShooterController>();
				}
				return m_controller;
			}
		}

		/// <summary>
		/// Indicates whether this component is preventing controller from firing
		/// </summary>
		public abstract bool canFire { get; }

		#endregion

		#region Constructors

		public ShooterBlockerData(MonoBehaviour owner)
			: base(owner)
		{ }

		#endregion
	}
}
