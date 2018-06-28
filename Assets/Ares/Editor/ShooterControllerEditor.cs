﻿using UnityEngine;
using UnityEditor;

namespace Ares
{
	[CustomEditor(typeof(ShooterController))]
	public class ShooterControllerEditor : AresEditor
	{
		#region Variables

		private ShooterController m_controller;
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

		#region Methods

		private void OnEnable()
		{
			m_controller = (ShooterController)target;
			m_isPlayerControlled = serializedObject.FindProperty("isPlayerControlled");

			// Controls
			m_isContinuous = serializedObject.FindProperty("isContinuous");
			m_isAutoFire = serializedObject.FindProperty("isAutoFire");
			m_timeBetweenShots = serializedObject.FindProperty("timeBetweenShots");
			m_fireOnButtonDown = serializedObject.FindProperty("fireOnButtonDown");
			m_fireButton = serializedObject.FindProperty("fireButton");

			// Burst Fire
			m_isBurstFire = serializedObject.FindProperty("isBurstFire");
			m_timeBetweenBursts = serializedObject.FindProperty("timeBetweenBursts");
			m_shotsPerBurst = serializedObject.FindProperty("shotsPerBurst");

			// Events
			m_onBeginFire = serializedObject.FindProperty("onBeginFire");
			m_onShotFiring = serializedObject.FindProperty("onShotFiring");
			m_onShotFired = serializedObject.FindProperty("onShotFired");
			m_onDryFired = serializedObject.FindProperty("onDryFired");
			m_onEndFire = serializedObject.FindProperty("onEndFire");

			// Properties
			m_shotsPerSecond = serializedObject.FindProperty("shotsPerSecond");
			m_damagePerSecond = serializedObject.FindProperty("damagePerSecond");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawBoxGroup(null, () =>
			{
				EditorGUILayout.PropertyField(m_isPlayerControlled);
			});

			DrawBoxGroup("Controls", DrawControlsGUI);
			DrawBoxGroup("Burst Fire", DrawBurstGUI);
			DrawFoldoutGroup(ref m_showEvents, "Events", DrawEventsGUI);

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.FloatField("Shots Per Second", m_controller.shotsPerSecond);
			EditorGUILayout.FloatField("Damage Per Second", m_controller.damagePerSecond);
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawControlsGUI()
		{
			EditorGUILayout.PropertyField(m_isContinuous);

			if (!m_isContinuous.boolValue)
			{
				EditorGUILayout.PropertyField(m_isAutoFire);

				if (m_isBurstFire.boolValue || m_controller.ammo == null || m_controller.ammo.useMagazine && m_controller.ammo.shotsPerMagazine > 1)
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
				if (m_controller.ammo == null || m_controller.ammo.useMagazine && m_controller.ammo.burstsPerMagazine > 1)
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