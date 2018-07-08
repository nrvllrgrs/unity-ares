using UnityEngine;
using Ares.Data;

namespace Ares
{
	public interface ICopyable<T>
	{
		void CopyTo(T dst);
	}
}