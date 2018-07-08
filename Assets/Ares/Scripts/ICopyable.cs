using UnityEngine;
using Ares.Data;

namespace Ares
{
	public interface ICopyable<T>
		where T : AresData
	{
		void CopyTo(T dst);
	}
}