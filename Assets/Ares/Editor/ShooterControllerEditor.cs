using UnityEngine;
using UnityEditor;
using Ares;
using Ares.Data;

namespace AresEditor
{
	[CustomEditor(typeof(ShooterController))]
	public class ShooterControllerEditor : Editor
	{
		#region Variables

		private ShooterController m_controller;
		private SerializedProperty m_data;

		private SerializedProperty m_isPlayerControlled;

		// Controls
		private SerializedProperty m_isContinuous;
		private SerializedProperty m_isAutoFire;
		private SerializedProperty m_timeBetweenShots;
		private SerializedProperty m_fireOnButtonDown;
		private SerializedProperty m_fireButton;

		// Burst Fire
		private SerializedProperty m_isBurstFire;
		private SerializedProperty m_timeBetweenBursts;
		private SerializedProperty m_shotsPerBurst;

		// Events
		private bool m_showEvents = false;
		private SerializedProperty m_onBeginFire;
		private SerializedProperty m_onShotFiring;
		private SerializedProperty m_onShotFired;
		private SerializedProperty m_onDryFired;
		private SerializedProperty m_onEndFire;

		// Derived
		private SerializedProperty m_shotsPerSecond;
		private SerializedProperty m_damagePerSecond;

		#endregion

		#region Properties

		private ShooterAmmoData ammo
		{
			get
			{
				if (m_controller == null)
					return null;

				if (m_controller.data.ammo == null)
					return null;

				return m_controller.data.ammo.data;
			}
		}

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_controller = (ShooterController)target;
			m_data = serializedObject.FindProperty("m_data");

			m_isPlayerControlled = m_data.FindPropertyRelative("isPlayerControlled");

			// Controls
			m_isContinuous = m_data.FindPropertyRelative("isContinuous");
			m_isAutoFire = m_data.FindPropertyRelative("isAutoFire");
			m_timeBetweenShots = m_data.FindPropertyRelative("timeBetweenShots");
			m_fireOnButtonDown = m_data.FindPropertyRelative("fireOnButtonDown");
			m_fireButton = m_data.FindPropertyRelative("fireButton");

			// Burst Fire
			m_isBurstFire = m_data.FindPropertyRelative("isBurstFire");
			m_timeBetweenBursts = m_data.FindPropertyRelative("timeBetweenBursts");
			m_shotsPerBurst = m_data.FindPropertyRelative("shotsPerBurst");

			// Events
			m_onBeginFire = m_data.FindPropertyRelative("onBeginFire");
			m_onShotFiring = m_data.FindPropertyRelative("onShotFiring");
			m_onShotFired = m_data.FindPropertyRelative("onShotFired");
			m_onDryFired = m_data.FindPropertyRelative("onDryFired");
			m_onEndFire = m_data.FindPropertyRelative("onEndFire");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AresEditorUtility.DrawBoxGroup(null, () =>
			{
				EditorGUILayout.PropertyField(m_isPlayerControlled);
			});

			AresEditorUtility.DrawBoxGroup("Controls", DrawControlsGUI);
			AresEditorUtility.DrawBoxGroup("Burst Fire", DrawBurstGUI);
			AresEditorUtility.DrawFoldoutGroup(ref m_showEvents, "Events", DrawEventsGUI);

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.FloatField("Shots Per Second", m_controller.data.shotsPerSecond);
			EditorGUILayout.FloatField("Damage Per Second", m_controller.data.damagePerSecond);
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawControlsGUI()
		{
			EditorGUILayout.PropertyField(m_isContinuous);

			if (!m_isContinuous.boolValue)
			{
				EditorGUILayout.PropertyField(m_isAutoFire);

				if (m_isBurstFire.boolValue || ammo == null || !ammo.useMagazine || ammo.shotsPerMagazine > 1)
				{
					if (m_timeBetweenShots.floatValue <= 0)
					{
						EditorGUILayout.HelpBox("\"Time Between Shots\" must be greater than 0!", MessageType.Error);
						m_timeBetweenShots.floatValue = 0f;
					}

					EditorGUILayout.PropertyField(m_timeBetweenShots);
				}

				if (m_isAutoFire.boolValue && !m_fireOnButtonDown.boolValue)
				{
					EditorGUILayout.HelpBox("\"Is Auto Fire\" and \"Fire On Button Down\" cannot be true and false respectively!", MessageType.Error);
				}

				EditorGUILayout.PropertyField(m_fireOnButtonDown);
				EditorGUILayout.PropertyField(m_fireButton);
			}
		}

		protected virtual void DrawBurstGUI()
		{
			EditorGUILayout.PropertyField(m_isBurstFire);
			if (m_isBurstFire.boolValue)
			{
				if (ammo == null || ammo.useMagazine && ammo.burstsPerMagazine > 1)
				{
					if (m_timeBetweenBursts.floatValue <= 0)
					{
						EditorGUILayout.HelpBox("\"Time Between Bursts\" must be greater than 0!", MessageType.Error);
						m_timeBetweenBursts.floatValue = 0f;
					}

					m_shotsPerBurst.intValue = Mathf.Max(m_shotsPerBurst.intValue, 2);

					EditorGUILayout.PropertyField(m_timeBetweenBursts);
					EditorGUILayout.PropertyField(m_shotsPerBurst);
				}
			}
		}

		protected virtual void DrawEventsGUI()
		{
			EditorGUILayout.PropertyField(m_onBeginFire);
			EditorGUILayout.PropertyField(m_onShotFiring);
			EditorGUILayout.PropertyField(m_onShotFired);
			EditorGUILayout.PropertyField(m_onDryFired);
			EditorGUILayout.PropertyField(m_onEndFire);
		}

		#endregion
	}
}
