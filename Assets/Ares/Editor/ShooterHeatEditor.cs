using UnityEngine;
using UnityEditor;
using Ares;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(ShooterHeat))]
	public class ShooterHeatEditor : ShooterHeatDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_heat = target as ShooterHeat;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(ShooterHeatNet))]
	public class ShooterHeatNetEditor : ShooterHeatDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_heat = target as ShooterHeatNet;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class ShooterHeatDataEditor : Editor
	{
		#region Variables

		protected IShooterHeat m_heat;
		private SerializedProperty m_data;

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

		protected virtual void OnEnable()
		{
			m_data = serializedObject.FindProperty("m_data");

			// Basic
			m_maxHeat = m_data.FindPropertyRelative("maxHeat");
			m_heatPerShot = m_data.FindPropertyRelative("heatPerShot");

			// Cooldown
			m_cooldownDelay = m_data.FindPropertyRelative("cooldownDelay");
			m_cooldownRate = m_data.FindPropertyRelative("cooldownRate");

			// Overheat
			m_overheatDelay = m_data.FindPropertyRelative("overheatDelay");
			m_overheatRate = m_data.FindPropertyRelative("overheatRate");

			// Events
			m_onHeatChanged = m_data.FindPropertyRelative("onHeatChanged");
			m_onBeginOverheat = m_data.FindPropertyRelative("onBeginOverheat");
			m_onEndOverheat = m_data.FindPropertyRelative("onEndOverheat");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AresEditorUtility.DrawBoxGroup(null, () =>
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

			AresEditorUtility.DrawBoxGroup("Cooldown", () =>
			{
				m_cooldownDelay.floatValue = Mathf.Max(m_cooldownDelay.floatValue, 0f);
				EditorGUILayout.PropertyField(m_cooldownDelay);

				m_cooldownRate.floatValue = Mathf.Max(m_cooldownRate.floatValue, 0f);
				EditorGUILayout.PropertyField(m_cooldownRate);
			});

			AresEditorUtility.DrawBoxGroup("Overheat", () =>
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
