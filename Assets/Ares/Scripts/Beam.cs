using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ares
{
	[RequireComponent(typeof(LineRenderer))]
	public class Beam : MonoBehaviour
	{
		#region Variables

		private LineRenderer m_lineRenderer;

		#endregion

		#region Properties

		public LineRenderer lineRenderer
		{
			get
			{
				if (m_lineRenderer == null)
				{
					m_lineRenderer = GetComponent<LineRenderer>();
				}
				return m_lineRenderer;
			}
		}

		#endregion
	}
}
