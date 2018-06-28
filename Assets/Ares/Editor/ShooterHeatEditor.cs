using UnityEngine;
using UnityEditor;

namespace Ares
{
	[CustomEditor(typeof(ShooterHeat))]
	public class ShooterHeatEditor : AresEditor
	{
		#region Variables

		private ShooterHeat m_heat;

		// Basic
		private SerializedProperty m_maxHeat;
		private SerializedProperty m_heatPerShot;

		// Cooldown
		private SerializedProperty m_cooldownDelay;
		private SerializedProperty m_cooldownRate;

		// Overheat
		private SerializedProperty m_overheatDelay;
		private SerializedProperty m_overheatRate;

		// Events
		private SerializedProperty m_onHeatChanged;
		private SerializedProperty m_onBeginOverheat;
		private SerializedProperty m_onEndOverheat;

		#endregion

		#region Methods

		private void OnEnable()
		{
			// Basic
			m_maxHeat = serializedObject.FindProperty("maxHeat");
			m_heatPerShot = serializedObject.FindProperty("heatPerShot");

			// Cooldown
			m_cooldownDelay = serializedObject.FindProperty("cooldownDelay");
			m_cooldownRate = serializedObject.FindProperty("cooldownRate");

			// Overheat
			m_overheatDelay = serializedObject.FindProperty("overheatDelay");
			m_overheatRate = serializedObject.FindProperty("overheatRate");

			// Events
			m_onHeatChanged = serializedObject.FindProperty("onHeatChanged");
			m_onBeginOverheat = serializedObject.FindProperty("onBeginOverheat");
			m_onEndOverheat = serializedObject.FindProperty("onEndOverheat");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawBoxGroup(null, () =>
			{
				if (m_maxHeat.floatValue <= 0f)
				{
					EditorGUILayout.HelpBox("\"Max Heat\" must be greater than 0!", MessageType.Error);
					m_maxHeat.floatValue = 0f;
				}

				EditorGUILayout.PropertyField(m_maxHeat);

				if (m_heatPerShot.floatValue <= 0f)
				{
					EditorGUILayout.HelpBox("\"Heat Per Shot\" must be greater than 0!", MessageType.Error);
					m_heatPerShot.floatValue = 0f;
				}

				EditorGUILayout.PropertyField(m_heatPerShot);
			});

			DrawBoxGroup("Cooldown", () =>
			{
				m_cooldownDelay.floatValue = Mathf.Max(m_cooldownDelay.floatValue, 0f);
				EditorGUILayout.PropertyField(m_cooldownDelay);

				m_cooldownRate.floatValue = Mathf.Max(m_cooldownRate.floatValue, 0f);
				EditorGUILayout.PropertyField(m_cooldownRate);
			});

			DrawBoxGroup("Overheat", () =>
			{
				m_overheatDelay.floatValue = Mathf.Max(m_overheatDelay.floatValue, 0f);
				EditorGUILayout.PropertyField(m_overheatDelay);

				m_overheatRate.floatValue = Mathf.Max(m_overheatRate.floatValue, 0f);
				EditorGUILayout.PropertyField(m_overheatRate);
			});

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
