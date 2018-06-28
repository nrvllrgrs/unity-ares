using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ares
{
	[CustomPropertyDrawer(typeof(DamageInfo))]
	public class DamageInfoDrawer : PropertyDrawer
	{
		#region Variables

		public static int PADDING = 2;

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			//EditorGUI.BeginProperty(position, label, property);

			// Draw label
			//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			DrawDamageGroup("Impact", "impact", position, property);
			//DrawDamageGroup("Splash", "splash", position, property);

			//EditorGUI.EndProperty();
		}

		private void DrawDamageGroup(string groupName, string propertyPrefix, Rect position, SerializedProperty property)
		{
			//GUILayout.BeginHorizontal(EditorStyles.helpBox);
			//GUILayout.BeginVertical();

			EditorGUI.BeginProperty(position, new GUIContent(groupName), property);
			EditorGUIUtility.LookLikeControls();

			var damageRect = EditorGUI.IndentedRect(position);
			var damage = property.FindPropertyRelative(string.Format("{0}Damage", propertyPrefix));
			EditorGUI.PropertyField(damageRect, damage);

			position.y += GetPropertyHeight(damage, new GUIContent("Damage")) + PADDING;

			if (damage.floatValue > 0f)
			{
				// Impulse
				var impulseRect = EditorGUI.IndentedRect(position);
				var impulse = property.FindPropertyRelative(string.Format("{0}Impulse", propertyPrefix));

				impulse.floatValue = Mathf.Max(impulse.floatValue, 0f);

				EditorGUI.PropertyField(impulseRect, impulse);
				position.y += GetPropertyHeight(impulse, new GUIContent("Damage")) + PADDING;

				// Range
				var rangeRect = EditorGUI.IndentedRect(position);
				var range = property.FindPropertyRelative(string.Format("{0}Range", propertyPrefix));

				range.floatValue = Mathf.Max(range.floatValue, 0f);

				EditorGUI.PropertyField(impulseRect, impulse);
				position.y += GetPropertyHeight(range, new GUIContent("Damage")) + PADDING;

				if (range.floatValue > 0f)
				{
					// Falloff
					var falloffRect = EditorGUI.IndentedRect(position);
					var falloff = property.FindPropertyRelative(string.Format("{0}Falloff", propertyPrefix));
					EditorGUI.PropertyField(falloffRect, falloff);
					position.y += GetPropertyHeight(falloff, new GUIContent("Damage")) + PADDING;
				}
			}

			EditorGUI.EndProperty();

			//GUILayout.EndVertical();
			//GUILayout.EndHorizontal();
		}

		//public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		//{
		//	return GetDamageGroupHeight(property, "impact", label)
		//		+ GetDamageGroupHeight(property, "splash", label)
		//		- PADDING;
		//}

		private float GetDamageGroupHeight(SerializedProperty property, string propertyPrefix, GUIContent label)
		{
			float height = 0f;

			var damage = property.FindPropertyRelative(string.Format("{0}Damage", propertyPrefix));
			height += GetPropertyHeight(damage, label) + PADDING;

			if (damage.floatValue > 0f)
			{
				var impulse = property.FindPropertyRelative(string.Format("{0}Impulse", propertyPrefix));
				height += GetPropertyHeight(impulse, label) + PADDING;

				var range = property.FindPropertyRelative(string.Format("{0}Range", propertyPrefix));
				height += GetPropertyHeight(range, label) + PADDING;

				if (range.floatValue > 0f)
				{
					var falloff = property.FindPropertyRelative(string.Format("{0}Falloff", propertyPrefix));
					height += GetPropertyHeight(falloff, label) + PADDING;
				}
			}

			return height;
		}

		#endregion
	}
}
