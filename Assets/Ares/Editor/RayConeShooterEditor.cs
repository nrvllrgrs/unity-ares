using UnityEngine;
using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(RayConeShooter))]
	public class RayConeShooterEditor : RayShooterEditor
	{
		#region Variables

		private RayConeShooter m_shooter;
		private SerializedProperty m_count;
		private SerializedProperty m_maxAngle;
		private SerializedProperty m_isNormalDistribution;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shooter = target as RayConeShooter;
			m_count = m_data.FindPropertyRelative("count");
			m_maxAngle = m_data.FindPropertyRelative("maxAngle");
			m_isNormalDistribution = m_data.FindPropertyRelative("isNormalDistribution");
		}

		protected override void DrawShooterGroup()
		{
			base.DrawShooterGroup();

			EditorGUILayout.PropertyField(m_count);
			EditorGUILayout.PropertyField(m_maxAngle);
			EditorGUILayout.PropertyField(m_isNormalDistribution);
		}

		#endregion
	}
}