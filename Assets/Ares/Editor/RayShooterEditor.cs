using UnityEditor;
using Ares;
using Ares.Networking;

namespace AresEditor
{
	[CustomEditor(typeof(RayShooter))]
	public class RayShooterEditor : RayShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as RayShooter;
			base.OnEnable();
		}

		#endregion
	}

	[CustomEditor(typeof(RayShooterNet))]
	public class RayShooterNetEditor : RayShooterDataEditor
	{
		#region Methods

		protected override void OnEnable()
		{
			m_shooter = target as RayShooterNet;
			base.OnEnable();
		}

		#endregion
	}

	public abstract class RayShooterDataEditor : ShooterEditor
	{
		#region Variables

		protected IShooter m_shooter;
		private SerializedProperty m_damage;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();
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
