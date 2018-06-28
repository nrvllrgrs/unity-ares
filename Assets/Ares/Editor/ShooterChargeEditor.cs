using UnityEditor;

namespace Ares
{
	[CustomEditor(typeof(ShooterCharge))]
	public class ShooterChargeEditor : AresEditor
	{
		#region Variables

		private ShooterCharge m_charge;

		private SerializedProperty m_maxCharge;
		private SerializedProperty m_resetOnEndFire;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_charge = (ShooterCharge)target;

			m_maxCharge = serializedObject.FindProperty("maxCharge");
			m_resetOnEndFire = serializedObject.FindProperty("resetOnEndFire");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawBoxGroup(null, () =>
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