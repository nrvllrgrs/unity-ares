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

		public Transform source { get; set; }

		#endregion

		#region Methods

		public void Refresh(Vector3 src, Vector3 dst)
		{
			Vector3 direction = (dst - src).normalized * Camera.main.farClipPlane;
			lineRenderer.SetPositions(new[]
			{
				src,
				src + direction
			});
		}

		#endregion
	}
}
