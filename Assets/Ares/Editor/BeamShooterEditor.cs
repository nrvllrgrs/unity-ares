using UnityEngine;
using UnityEditor;
using Ares;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(BeamShooter))]
	public class BeamShooterEditor : BeamShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as BeamShooter;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(BeamShooterNet))]
	public class BeamShooterNetEditor : BeamShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as BeamShooterNet;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class BeamShooterDataEditor : RayShooterDataEditor
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