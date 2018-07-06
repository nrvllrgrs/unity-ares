using Ares.Data;

namespace Ares
{
	public interface IShooter
	{
		#region Properties

		ShooterData shooterData { get; }

		#endregion
	}

	public interface IShooter<T>
		where T : ShooterData
	{
		#region Properties

		T data { get; }

		#endregion
	}
}
