using UnityEngine;
using Ares.Data;

namespace Ares
{
	public interface IHealth
	{
		#region Properties

		HealthData data { get; }

		// Include implicit MonoBehaviour properties
		Transform transform { get; }
		GameObject gameObject { get; }

		#endregion
	}
}
