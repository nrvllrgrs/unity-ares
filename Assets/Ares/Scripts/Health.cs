using Ares.Data;

namespace Ares
{
	public class Health : AresMonoBehaviour<HealthData>, IHealth
	{
		#region Methods

		protected override void Reset()
		{
			data = new HealthData(this);
		}

		#endregion
	}
}