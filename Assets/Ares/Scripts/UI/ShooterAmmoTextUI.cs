using UnityEngine;
using TMPro;

namespace Ares.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ShooterAmmoTextUI : MonoBehaviour
	{
		#region Variables

		public IShooterAmmo ammo;

		private TextMeshProUGUI m_text;

		#endregion

		#region Properties

		public TextMeshProUGUI text
		{
			get
			{
				if (m_text == null)
				{
					m_text = GetComponent<TextMeshProUGUI>();
				}
				return m_text;
			}
		}

		#endregion

		#region Methods

		private void Start()
		{
			if (ammo == null)
			{
				Debug.LogError(string.Format("Ammo is undefined for {0}!", name));
				return;
			}

			if (ammo.data.isInfinite && !ammo.data.useMagazine)
			{
				text.text = "∞";
			}
			else
			{
				ammo.data.controller.data.onShotFired.AddListener(OnShotFired);
				OnShotFired();
			}
		}

		private void OnShotFired()
		{
			if (!ammo.data.useMagazine)
			{
				if (!ammo.data.isInfinite)
				{
					text.text = ammo.data.count.ToString();
				}
			}
			else
			{
				if (!ammo.data.isInfinite)
				{
					text.text = string.Format(
						"{0}<size={2}> </size>/<size={2}> {1}</size>",
						ammo.data.shotsInMagazine.ToString(),
						ammo.data.count.ToString(),
						text.fontSize / 2);
				}
				else
				{
					text.text = ammo.data.shotsInMagazine.ToString();
				}
			}
		}

		#endregion
	}
}
