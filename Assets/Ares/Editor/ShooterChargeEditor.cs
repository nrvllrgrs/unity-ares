using UnityEditor;
using Ares;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(ShooterCharge))]
	public class ShooterChargeEditor : ShooterChargeDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_charge = target as ShooterCharge;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(ShooterChargeNet))]
	public class ShooterChargeNetEditor : ShooterChargeDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_charge = target as ShooterChargeNet;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class ShooterChargeDataEditor : Editor
	{
		#region Variables

		protected IShooterCharge m_charge;
		private SerializedProperty m_data;

		private SerializedProperty m_maxCharge;
		private SerializedProperty m_resetOnEndFire;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_data = serializedObject.FindProperty("m_data");

			m_maxCharge = m_data.FindPropertyRelative("maxCharge");
			m_resetOnEndFire = m_data.FindPropertyRelative("resetOnEndFire");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AresEditorUtility.DrawBoxGroup(null, () =>
			{
				if (m_maxCharge.floatValue <= 0f)
				{
					EditorGUILayout.HelpBox("\"Max Charge\" must be greater than 0!", MessageType.Error);
					m_maxCharge.floatValue = 0f;
				}

				EditorGUILayout.PropertyField(m_maxCharge);
				EditorGUILayout.PropertyField(m_resetOnEndFire);
			});

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}