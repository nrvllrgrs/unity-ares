using UnityEngine;
using UnityEditor;

namespace Ares
{
	[CustomEditor(typeof(ShooterAmmo))]
	public class ShooterAmmoEditor : AresEditor
	{
		#region Variables

		private ShooterAmmo m_ammo;

		// Basic
		private SerializedProperty m_isInfinite;
		private SerializedProperty m_capacity;
		private SerializedProperty m_count;

		// Magazine
		private SerializedProperty m_useMagazine;
		private SerializedProperty m_magazineCapacity;
		private SerializedProperty m_magazineCount;
		private SerializedProperty m_shotsPerMagazine;
		private SerializedProperty m_shotsInMagazine;
		private SerializedProperty m_burstsPerMagazine;
		private SerializedProperty m_isAutoReload;
		private SerializedProperty m_isSimultaneousReload;
		private SerializedProperty m_reloadTime;
		private SerializedProperty m_consecutiveReloadTime;
		private SerializedProperty m_reloadButton;

		// Regeneration
		private SerializedProperty m_useRegeneration;
		private SerializedProperty m_regenerationDelay;
		private SerializedProperty m_regenerationRate;

		// Events
		private bool m_showEvents;
		private SerializedProperty m_onCountChanged;
		private SerializedProperty m_onBeginReload;
		private SerializedProperty m_onEndReload;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_ammo = (ShooterAmmo)target;

			// Basic
			m_isInfinite = serializedObject.FindProperty("isInfinite");
			m_capacity = serializedObject.FindProperty("capacity");
			m_count = serializedObject.FindProperty("m_count");

			// Magazine
			m_useMagazine = serializedObject.FindProperty("useMagazine");
			m_magazineCapacity = serializedObject.FindProperty("magazineCapacity");
			m_magazineCount = serializedObject.FindProperty("magazineCount");
			m_shotsPerMagazine = serializedObject.FindProperty("shotsPerMagazine");
			m_shotsInMagazine = serializedObject.FindProperty("m_shotsInMagazine");
			m_isAutoReload = serializedObject.FindProperty("isAutoReload");
			m_isSimultaneousReload = serializedObject.FindProperty("isSimultaneousReload");
			m_reloadTime = serializedObject.FindProperty("reloadTime");
			m_consecutiveReloadTime = serializedObject.FindProperty("consecutiveReloadTime");
			m_reloadButton = serializedObject.FindProperty("reloadButton");

			//Regeneration
			m_useRegeneration = serializedObject.FindProperty("useRegeneration");
			m_regenerationDelay = serializedObject.FindProperty("regenerationDelay");
			m_regenerationRate = serializedObject.FindProperty("regenerationRate");

			// Events
			m_onCountChanged = serializedObject.FindProperty("onCountChanged");
			m_onBeginReload = serializedObject.FindProperty("onBeginReload");
			m_onEndReload = serializedObject.FindProperty("onEndReload");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (m_isInfinite.boolValue && !m_useMagazine.boolValue)
			{
				EditorGUILayout.HelpBox("If \"Is Infinite\" and \"Use Magazine\" are true and false respectively, then \"Shooter Ammo\" component is not required!", MessageType.Warning);
			}

			DrawBoxGroup(null, () =>
			{
				EditorGUILayout.PropertyField(m_isInfinite);

				if (!m_isInfinite.boolValue)
				{
					if (m_useMagazine.boolValue)
					{
						m_capacity.intValue = m_magazineCapacity.intValue * m_shotsPerMagazine.intValue;
						m_count.intValue = m_magazineCount.intValue * (m_shotsPerMagazine.intValue - 1) + m_shotsInMagazine.intValue;
					}

					m_capacity.intValue = Mathf.Max(m_capacity.intValue, 0);
					m_count.intValue = Mathf.Clamp(m_count.intValue, 0, m_capacity.intValue);

					EditorGUI.BeginDisabledGroup(m_useMagazine.boolValue);
					EditorGUILayout.PropertyField(m_capacity);
					EditorGUILayout.PropertyField(m_count);
					EditorGUI.EndDisabledGroup();
				}
			});

			DrawBoxGroup("Magazine", () =>
			{
				EditorGUILayout.PropertyField(m_useMagazine);

				if (m_useMagazine.boolValue)
				{
					if (!m_isInfinite.boolValue)
					{
						m_magazineCount.intValue = Mathf.Clamp(m_magazineCount.intValue, 0, m_magazineCapacity.intValue);

						EditorGUILayout.PropertyField(m_magazineCapacity);
						EditorGUILayout.PropertyField(m_magazineCount);
					}

					m_shotsInMagazine.intValue = Mathf.Clamp(m_shotsInMagazine.intValue, 0, m_shotsPerMagazine.intValue);

					if (m_ammo.controller.isBurstFire
						&& m_ammo.controller.shotsPerBurst > 0
						&& m_shotsPerMagazine.intValue % m_ammo.controller.shotsPerBurst != 0)
					{
						EditorGUILayout.HelpBox("\"Shots Per Magazine\" is not divisible by \"Shots Per Burst\"!", MessageType.Error);
					}

					if (m_shotsPerMagazine.intValue <= 0)
					{
						EditorGUILayout.HelpBox("\"Shots Per Magazine\" must be greater than 0!", MessageType.Error);
						m_shotsPerMagazine.intValue = 0;
					}

					EditorGUILayout.PropertyField(m_shotsPerMagazine);
					EditorGUILayout.PropertyField(m_shotsInMagazine);

					if (m_ammo.controller.isBurstFire)
					{
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.FloatField("Bursts Per Magazine", m_ammo.burstsPerMagazine);
						EditorGUI.EndDisabledGroup();
					}

					EditorGUILayout.PropertyField(m_isAutoReload);
					EditorGUILayout.PropertyField(m_isSimultaneousReload);

					if (m_reloadTime.floatValue <= 0f)
					{
						EditorGUILayout.HelpBox("\"Reload Time\" must be greater than 0!", MessageType.Error);
						m_reloadTime.floatValue = 0f;
					}

					EditorGUILayout.PropertyField(m_reloadTime);

					if (!m_isSimultaneousReload.boolValue)
					{
						if (m_consecutiveReloadTime.floatValue <= 0f)
						{
							EditorGUILayout.HelpBox("\"Consecutive Reload Time\" must be greater than 0!", MessageType.Error);
							m_consecutiveReloadTime.floatValue = 0f;
						}

						EditorGUILayout.PropertyField(m_consecutiveReloadTime);
					}

					EditorGUILayout.PropertyField(m_reloadButton);
				}
			});

			if (!m_isInfinite.boolValue)
			{
				DrawBoxGroup("Regeneration", () =>
				{
					EditorGUILayout.PropertyField(m_useRegeneration);
					
					if (m_useRegeneration.boolValue)
					{
						m_regenerationDelay.floatValue = Mathf.Max(m_regenerationDelay.floatValue, 0f);
						m_regenerationRate.floatValue = Mathf.Max(m_regenerationRate.floatValue, 0f);

						EditorGUILayout.PropertyField(m_regenerationDelay);
						EditorGUILayout.PropertyField(m_regenerationRate);
					}
				});
			}

			DrawFoldoutGroup(ref m_showEvents, "Events", () =>
			{
				EditorGUILayout.PropertyField(m_onCountChanged);

				// Do not show reload events if there is no reload action
				if (m_useMagazine.boolValue)
				{
					EditorGUILayout.PropertyField(m_onBeginReload);
					EditorGUILayout.PropertyField(m_onEndReload);
				}
			});

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
