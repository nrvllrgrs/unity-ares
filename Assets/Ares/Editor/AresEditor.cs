using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ares
{
	public abstract class AresEditor : Editor
	{
		#region Methods

		protected void DrawBoxGroup(string groupName, System.Action action)
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

		protected void DrawFoldoutGroup(ref bool showGroup, string groupName, System.Action action)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			showGroup = EditorGUILayout.Foldout(showGroup, groupName, true);
			EditorGUILayout.EndHorizontal();

			if (showGroup && action != null)
			{
				action();
			}

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		#endregion
	}
}
