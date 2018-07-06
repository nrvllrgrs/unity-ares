using Ares.Data;

namespace Ares
{
	public interface IShooterBlocker
	{
		#region Properties

		ShooterBlockerData blockerData { get; }

		#endregion
	}

	public interface IShooterBlocker<T>
		where T : ShooterBlockerData
	{
		#region Properties

		T data { get; }

		#endregion
	}
}
