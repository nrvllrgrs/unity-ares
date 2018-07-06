using UnityEngine;
using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(BeamShooter))]
	public class BeamShooterEditor : RayShooterEditor
	{
		#region Variables

		private BeamShooter m_shooter;
		private SerializedProperty m_beamTemplate;
		private SerializedProperty m_endBeamTemplate;
		private SerializedProperty m_spawnOnShotFired;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shooter = target as BeamShooter;
			m_beamTemplate = m_data.FindPropertyRelative("beamTemplate");
			m_endBeamTemplate = m_data.FindPropertyRelative("endBeamTemplate");
			m_spawnOnShotFired = m_data.FindPropertyRelative("spawnOnShotFired");
		}

		protected override void DrawShooterGroup()
		{
			base.DrawShooterGroup();

			EditorGUILayout.PropertyField(m_beamTemplate);
			EditorGUILayout.PropertyField(m_endBeamTemplate);
			EditorGUILayout.PropertyField(m_spawnOnShotFired);
		}

		#endregion
	}
}