using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Ares;

namespace AresEditor
{
	[CustomEditor(typeof(RayShooter))]
	public class RayShooterEditor : ShooterEditor
	{
		#region Variables

		private IShooter m_shooter;
		private SerializedProperty m_damage;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shooter = target as RayShooter;
			m_damage = m_data.FindPropertyRelative("damage");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();
			AresEditorUtility.DrawDamageGroup(m_damage);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
