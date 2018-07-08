using UnityEngine;
using UnityEngine.UI;

namespace Ares.UI
{
	[RequireComponent(typeof(Slider))]
	public class ShooterHeatSliderUI : MonoBehaviour
	{
		#region Variables

		public IShooterHeat heat;
		public Image fillArea;

		[SerializeField]
		public Gradient gradientColor;

		private Slider m_slider;

		#endregion

		#region Properties

		public Slider slider
		{
			get
			{
				if (m_slider == null)
				{
					m_slider = GetComponent<Slider>();
				}
				return m_slider;
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			if (heat == null)
			{
				Debug.LogError(string.Format("Heat is undefined for {0}!", name));
				return;
			}

			heat.data.onHeatChanged.AddListener(OnHeatChanged);
			OnHeatChanged();

			heat.data.onBeginOverheat.AddListener(() =>
			{
				fillArea.color = gradientColor.Evaluate(1f);
			});

			heat.data.onEndOverheat.AddListener(() =>
			{
				fillArea.color = gradientColor.Evaluate(0f);
			});
		}

		private void OnHeatChanged()
		{
			slider.value = heat.data.percent;
			if (!heat.data.isOverheated)
			{
				fillArea.color = gradientColor.Evaluate(heat.data.percent);
			}
		}

		#endregion
	}
}