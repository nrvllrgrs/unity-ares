using UnityEngine;
using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(Projectile))]
	public class ProjectileEditor : Editor
	{
		#region Variables

		private Projectile m_projectile;
		protected SerializedProperty m_data;

		private SerializedProperty m_damage;

		//public CollisionDetonationType collisionDetonationType;

		// Physics
		private SerializedProperty m_speed;
		private SerializedProperty m_acceleration;
		private SerializedProperty m_maxSpeed;

		// Tracking
		private SerializedProperty m_target;
		private SerializedProperty m_angularSpeed;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_projectile = (Projectile)target;
			m_data = serializedObject.FindProperty("m_data");

			m_damage = m_data.FindPropertyRelative("damage");

			// Physics
			m_speed = m_data.FindPropertyRelative("speed");
			m_acceleration = m_data.FindPropertyRelative("acceleration");
			m_maxSpeed = m_data.FindPropertyRelative("maxSpeed");

			// Tracking
			m_target = m_data.FindPropertyRelative("target");
			m_angularSpeed = m_data.FindPropertyRelative("angularSpeed");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AresEditorUtility.DrawDamageGroup(m_damage);

			AresEditorUtility.DrawBoxGroup("Physics", () =>
			{
				if (m_speed.floatValue <= 0)
				{
					EditorGUILayout.HelpBox("\"Speed\" must be greater than 0!", MessageType.Error);
					m_speed.floatValue = 0f;
				}

				EditorGUILayout.PropertyField(m_speed);

				m_acceleration.floatValue = Mathf.Max(m_acceleration.floatValue, 0f);
				EditorGUILayout.PropertyField(m_acceleration);

				if (m_acceleration.floatValue > 0f)
				{
					m_maxSpeed.floatValue = Mathf.Min(m_maxSpeed.floatValue, m_speed.floatValue);
					EditorGUILayout.PropertyField(m_maxSpeed);
				}
			});

			AresEditorUtility.DrawBoxGroup("Tracking", () =>
			{
				EditorGUILayout.PropertyField(m_target);

				if (m_angularSpeed.floatValue <= 0f && m_target.objectReferenceValue != null)
				{
					EditorGUILayout.HelpBox("\"Angular Speed\" must be greater than 0!", MessageType.Error);
					m_angularSpeed.floatValue = 0f;
				}

				EditorGUILayout.PropertyField(m_angularSpeed);
			});

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
