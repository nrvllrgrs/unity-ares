using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.Reflection;

using Ares;
using Ares.Networking;
using Ares.Data;

namespace AresEditor
{
	public static class AresEditorUtility
	{
		#region Static Methods

		[MenuItem("Tools/Ares/Convert to NetworkBehaviour")]
		public static void ConvertToNetworkBehaviour()
		{
			GameObject[] objects = Selection.gameObjects;
			foreach (var src in objects)
			{
				GameObject dst = Object.Instantiate(src);
				dst.name = string.Format("{0} - Net", src.name);

				ConvertToNetworkBehaviour<ShooterController, ShooterControllerNet, ShooterControllerData>(dst);
				ConvertToNetworkBehaviour<ShooterAmmo, ShooterAmmoNet, ShooterAmmoData>(dst);
				ConvertToNetworkBehaviour<ShooterCharge, ShooterChargeNet, ShooterChargeData>(dst);
				ConvertToNetworkBehaviour<ShooterHeat, ShooterHeatNet, ShooterHeatData>(dst);
				ConvertToNetworkBehaviour<RayShooter, RayShooterNet, RayShooterData>(dst);
				ConvertToNetworkBehaviour<RayConeShooter, RayConeShooterNet, BeamShooterData>(dst);
				ConvertToNetworkBehaviour<BeamShooter, BeamShooterNet, BeamShooterData>(dst);
				ConvertToNetworkBehaviour<ProjectileShooter, ProjectileShooterNet, ProjectileShooterData>(dst);
				ConvertToNetworkBehaviour<Projectile, ProjectileNet, ProjectileData>(dst);
				ConvertToNetworkBehaviour<Health, HealthNet, HealthData>(dst);

				// Cleanup components that were required by other components
				RemoveComponents<ShooterController>(dst);
			}
		}

		private static void ConvertToNetworkBehaviour<T, K, J>(GameObject obj)
			where T : MonoBehaviour
			where K : NetworkBehaviour
			where J : AresData
		{
			var childrenComponents = obj.GetComponentsInChildren<T>();
			foreach (var srcComponent in childrenComponents)
			{
				J srcData = GetData(srcComponent) as J;
				if (srcData == null)
					continue;

				var srcCopyable = srcData as ICopyable<J>;
				if (srcCopyable == null)
					continue;

				var dstComponent = obj.AddComponent<K>();
				J dstData = GetData(dstComponent) as J;
				if (dstData == null)
				{
					Object.Destroy(dstComponent);
					continue;
				}

				srcCopyable.CopyTo(dstData);

				// Remove source component
				Object.DestroyImmediate(srcComponent);
			}
		}

		private static void RemoveComponents<T>(GameObject obj)
			where T : MonoBehaviour
		{
			foreach (var comp in obj.GetComponentsInChildren<T>())
			{
				Object.DestroyImmediate(comp);
			}
		}

		private static AresData GetData(MonoBehaviour behaviour)
		{
			return (AresData)behaviour.GetType().GetProperty("data").GetValue(behaviour, null);
		}

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