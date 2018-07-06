using UnityEditor;
using Ares;

namespace AresEditor
{
	public abstract class ShooterEditor : Editor
	{
		#region Variables

		protected SerializedProperty m_data;
		private SerializedProperty m_muzzleBone;

		// Audio
		private SerializedProperty m_clip;
		private SerializedProperty m_volume;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_data = serializedObject.FindProperty("m_data");
			m_muzzleBone = m_data.FindPropertyRelative("muzzleBone");

			// Audio
			m_clip = m_data.FindPropertyRelative("clip");
			m_volume = m_data.FindPropertyRelative("volume");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AresEditorUtility.DrawBoxGroup(null, DrawShooterGroup);
			AresEditorUtility.DrawBoxGroup("Audio", DrawAudioGroup);

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawShooterGroup()
		{
			EditorGUILayout.PropertyField(m_muzzleBone);
		}

		protected void DrawAudioGroup()
		{
			EditorGUILayout.PropertyField(m_clip);
			EditorGUILayout.Slider(m_volume, 0f, 1f);
		}

		#endregion
	}
}