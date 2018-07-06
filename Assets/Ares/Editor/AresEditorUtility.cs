using UnityEngine;
using UnityEditor;

namespace AresEditor
{
	public static class AresEditorUtility
	{
		#region Static Methods

		public static void DrawBoxGroup(string groupName, System.Action action)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			EditorGUILayout.BeginVertical();

			if (!string.IsNullOrEmpty(groupName))
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
				EditorGUILayout.LabelField(groupName, EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
			}

			if (action != null)
			{
				action();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		public static void DrawFoldoutGroup(ref bool showGroup, string groupName, System.Action action)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			EditorGUILayout.BeginVertical();

			if (!string.IsNullOrEmpty(groupName))
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
				showGroup = EditorGUILayout.Foldout(showGroup, groupName);
				EditorGUILayout.EndHorizontal();
			}

			if (showGroup && action != null)
			{
				action();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		public static void DrawDamageGroup(SerializedProperty damageInfo)
		{
			DrawBoxGroup(null, () =>
			{
				DrawDamageGroup(damageInfo, "Impact", "impact");
				DrawDamageGroup(damageInfo, "Splash", "splash");

				var layerMask = damageInfo.FindPropertyRelative("layerMask");
				EditorGUILayout.PropertyField(layerMask);
			});
		}

		private static void DrawDamageGroup(SerializedProperty damageInfo, string groupName, string prefix)
		{
			DrawBoxGroup(groupName, () =>
			{
				var damage = damageInfo.FindPropertyRelative(string.Format("{0}Damage", prefix));
				damage.floatValue = Mathf.Max(damage.floatValue, 0f);
				EditorGUILayout.PropertyField(damage, new GUIContent("Damage"));

				if (damage.floatValue > 0f)
				{
					var impulse = damageInfo.FindPropertyRelative(string.Format("{0}Impulse", prefix));
					impulse.floatValue = Mathf.Max(impulse.floatValue, 0f);
					EditorGUILayout.PropertyField(impulse, new GUIContent("Impulse"));
				}

				var range = damageInfo.FindPropertyRelative(string.Format("{0}Range", prefix));
				if (range.floatValue == 0f)
				{
					EditorGUILayout.HelpBox("\"Range\" is infinite.", MessageType.Info);
				}
				range.floatValue = Mathf.Max(range.floatValue, 0f);
				EditorGUILayout.PropertyField(range, new GUIContent("Range"));

				if (damage.floatValue > 0f && range.floatValue > 0f)
				{
					var falloff = damageInfo.FindPropertyRelative(string.Format("{0}Falloff", prefix));
					EditorGUILayout.PropertyField(falloff, new GUIContent("Falloff"));
				}
			});
		}

		#endregion
	}
}