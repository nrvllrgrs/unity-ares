using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(ShooterCharge))]
	public class ShooterChargeEditor : Editor
	{
		#region Variables

		private ShooterCharge m_charge;
		private SerializedProperty m_data;

		private SerializedProperty m_maxCharge;
		private SerializedProperty m_resetOnEndFire;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_charge = (ShooterCharge)target;
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