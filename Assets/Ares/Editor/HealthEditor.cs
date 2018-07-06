using UnityEngine;
using UnityEditor;
using Ares;
using Ares.Data;

namespace AresEditor
{
	[CustomEditor(typeof(Health))]
	public class HealthEditor : Editor
	{
		#region Variables

		private Health m_health;
		private SerializedProperty m_data;

		private SerializedProperty m_maxPoints;
		private SerializedProperty m_points;
		private SerializedProperty m_destroyOnKilled;

		// Regeneration
		private SerializedProperty m_regenerationDelay;
		private SerializedProperty m_regenerationRate;

		// Invulnerability
		private SerializedProperty m_invulnerabilityTime;

		// Events
		private SerializedProperty m_onHealthChanged;
		private SerializedProperty m_onKilled;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_health = (Health)target;
			m_data = serializedObject.FindProperty("m_data");

			m_maxPoints = m_data.FindPropertyRelative("maxHealth");
			m_points = m_data.FindPropertyRelative("m_health");
			m_destroyOnKilled = m_data.FindPropertyRelative("destroyOnKilled");

			// Regeneration
			m_regenerationDelay = m_data.FindPropertyRelative("regenerationDelay");
			m_regenerationRate = m_data.FindPropertyRelative("regenerationRate");

			// Invulnerability
			m_invulnerabilityTime = m_data.FindPropertyRelative("invulnerabilityTime");

			// Events
			m_onHealthChanged = m_data.FindPropertyRelative("onHealthChanged");
			m_onKilled = m_data.FindPropertyRelative("onKilled");
		}

		public override void OnInspectorGUI()
		{
			AresEditorUtility.DrawBoxGroup(null, () =>
			{
				if (m_maxPoints.floatValue <= 0f)
				{
					EditorGUILayout.HelpBox("\"Max Health\" must be greater than 0!", MessageType.Error);
					m_maxPoints.floatValue = 0f;
				}
				EditorGUILayout.PropertyField(m_maxPoints);

				m_points.floatValue = Mathf.Clamp(m_points.floatValue, 0f, m_maxPoints.floatValue);
				EditorGUILayout.PropertyField(m_points);
				EditorGUILayout.PropertyField(m_destroyOnKilled);
			});

			AresEditorUtility.DrawBoxGroup("Regeneration", () =>
			{
				m_regenerationDelay.floatValue = Mathf.Max(m_regenerationDelay.floatValue, 0f);
				EditorGUILayout.PropertyField(m_regenerationDelay);

				if (m_regenerationRate.floatValue == 0f)
				{
					EditorGUILayout.HelpBox("\"Regeneration Rate\" is 0! Are you changing this value at runtime?", MessageType.Warning);
				}
				EditorGUILayout.PropertyField(m_regenerationRate);
			});

			AresEditorUtility.DrawBoxGroup("Invulnerability", () =>
			{
				m_invulnerabilityTime.floatValue = Mathf.Max(m_invulnerabilityTime.floatValue, 0f);
				EditorGUILayout.PropertyField(m_invulnerabilityTime);
			});
		}

		#endregion
	}
}