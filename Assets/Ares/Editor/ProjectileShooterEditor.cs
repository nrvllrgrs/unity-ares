﻿using UnityEngine;
using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(ProjectileShooter))]
	public class ProjectileShooterEditor : ShooterEditor
	{
		#region Variables

		private ProjectileShooter m_shooter;
		private SerializedProperty m_projectileTemplate;

		private SerializedProperty m_relProjectileTemplate;

		// Controls
		private SerializedProperty m_spawnOnBeginFire;

		// Tracking
		private SerializedProperty m_canTrack;
		private SerializedProperty m_maxLockTime;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shooter = (ProjectileShooter)target;
			m_projectileTemplate = serializedObject.FindProperty("projectileTemplate");

			m_relProjectileTemplate = m_data.FindPropertyRelative("projectileTemplate");
			m_canTrack = m_data.FindPropertyRelative("canTrack");
			m_maxLockTime = m_data.FindPropertyRelative("maxLockTime");
		}

		protected override void DrawShooterGroup()
		{
			base.DrawShooterGroup();

			if (m_projectileTemplate.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("\"Projectile Template\" required!", MessageType.Error);
			}

			EditorGUILayout.PropertyField(m_projectileTemplate);
			m_relProjectileTemplate.objectReferenceValue = m_projectileTemplate.objectReferenceValue;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			AresEditorUtility.DrawBoxGroup("Tracking", () =>
			{
				EditorGUILayout.PropertyField(m_canTrack);

				if (m_canTrack.boolValue)
				{
					m_maxLockTime.floatValue = Mathf.Max(m_maxLockTime.floatValue, 0f);
					EditorGUILayout.PropertyField(m_maxLockTime);
				}
			});

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}