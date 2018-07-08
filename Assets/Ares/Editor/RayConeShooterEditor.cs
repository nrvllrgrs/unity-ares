using UnityEditor;
using Ares;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(RayConeShooter))]
	public class RayConeShooterEditor : RayConeShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as RayConeShooter;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(RayConeShooterNet))]
	public class RayConeShooterNetEditor : RayConeShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as RayConeShooterNet;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class RayConeShooterDataEditor : RayShooterDataEditor
	{
		#region Variables

		private SerializedProperty m_count;
		private SerializedProperty m_maxAngle;
		private SerializedProperty m_isNormalDistribution;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shooter = target as RayConeShooter;
			m_count = m_data.FindPropertyRelative("count");
			m_maxAngle = m_data.FindPropertyRelative("maxAngle");
			m_isNormalDistribution = m_data.FindPropertyRelative("isNormalDistribution");
		}

		protected override void DrawShooterGroup()
		{
			base.DrawShooterGroup();

			EditorGUILayout.PropertyField(m_count);
			EditorGUILayout.PropertyField(m_maxAngle);
			EditorGUILayout.PropertyField(m_isNormalDistribution);
		}

		#endregion
	}
}