using UnityEngine;
using UnityEditor;
using Ares;
using Ares.Data;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(ShooterAmmo))]
	public class ShooterAmmoEditor : ShooterAmmoDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_ammo = (ShooterAmmo)target;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(ShooterAmmoNet))]
	public class ShooterAmmoNetEditor : ShooterAmmoDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_ammo = (ShooterAmmoNet)target;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class ShooterAmmoDataEditor : Editor
	{
		#region Variables

		protected IShooterAmmo m_ammo;
		private SerializedProperty m_data;

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

		#region Properties

		private ShooterControllerData controller
		{
			get
			{
				if (m_ammo == null)
					return null;

				if (m_ammo.data.controller == null)
					return null;

				return m_ammo.data.controller.data;
			}
		}

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_data = serializedObject.FindProperty("m_data");

			// Basic
			m_isInfinite = m_data.FindPropertyRelative("isInfinite");
			m_capacity = m_data.FindPropertyRelative("capacity");
			m_count = m_data.FindPropertyRelative("m_count");

			// Magazine
			m_useMagazine = m_data.FindPropertyRelative("useMagazine");
			m_magazineCapacity = m_data.FindPropertyRelative("magazineCapacity");
			m_magazineCount = m_data.FindPropertyRelative("magazineCount");
			m_shotsPerMagazine = m_data.FindPropertyRelative("shotsPerMagazine");
			m_shotsInMagazine = m_data.FindPropertyRelative("m_shotsInMagazine");
			m_isAutoReload = m_data.FindPropertyRelative("isAutoReload");
			m_isSimultaneousReload = m_data.FindPropertyRelative("isSimultaneousReload");
			m_reloadTime = m_data.FindPropertyRelative("reloadTime");
			m_consecutiveReloadTime = m_data.FindPropertyRelative("consecutiveReloadTime");
			m_reloadButton = m_data.FindPropertyRelative("reloadButton");

			//Regeneration
			m_useRegeneration = m_data.FindPropertyRelative("useRegeneration");
			m_regenerationDelay = m_data.FindPropertyRelative("regenerationDelay");
			m_regenerationRate = m_data.FindPropertyRelative("regenerationRate");

			// Events
			m_onCountChanged = m_data.FindPropertyRelative("onCountChanged");
			m_onBeginReload = m_data.FindPropertyRelative("onBeginReload");
			m_onEndReload = m_data.FindPropertyRelative("onEndReload");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (m_isInfinite.boolValue && !m_useMagazine.boolValue)
			{
				EditorGUILayout.HelpBox("If \"Is Infinite\" and \"Use Magazine\" are true and false respectively, then \"Shooter Ammo\" component is not required!", MessageType.Warning);
			}

			AresEditorUtility.DrawBoxGroup(null, () =>
			{
				EditorGUILayout.PropertyField(m_isInfinite);

				if (!m_isInfinite.boolValue)
				{
					if (m_useMagazine.boolValue)
					{
						m_capacity.intValue = m_magazineCapacity.intValue * m_shotsPerMagazine.intValue;
						m_count.intValue = m_magazineCount.intValue * (m_shotsPerMagazine.intValue - 1) + m_shotsInMagazine.intValue;
					}

					if (m_capacity.intValue <= 0)
					{
						EditorGUILayout.HelpBox("\"Capacity\" must be greater than 0!", MessageType.Error);
						m_capacity.intValue = 0;
					}

					m_count.intValue = Mathf.Clamp(m_count.intValue, 0, m_capacity.intValue);

					EditorGUI.BeginDisabledGroup(m_useMagazine.boolValue);
					EditorGUILayout.PropertyField(m_capacity);
					EditorGUILayout.PropertyField(m_count);
					EditorGUI.EndDisabledGroup();
				}
			});

			AresEditorUtility.DrawBoxGroup("Magazine", () =>
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

					if (controller != null
						&& controller.isBurstFire
						&& controller.shotsPerBurst > 0
						&& m_shotsPerMagazine.intValue % controller.shotsPerBurst != 0)
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

					if (controller != null && controller.isBurstFire)
					{
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.FloatField("Bursts Per Magazine", m_ammo.data.burstsPerMagazine);
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
				AresEditorUtility.DrawBoxGroup("Regeneration", () =>
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

			AresEditorUtility.DrawFoldoutGroup(ref m_showEvents, "Events", () =>
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
