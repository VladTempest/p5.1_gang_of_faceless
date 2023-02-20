using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Editor.Scripts.MainMenuScripts.UI
{
	public class ChangeLocalizationUI : MonoBehaviour
	{
		private List<Locale> _locale;
		private List<Locale> Locale => _locale ??= LocalizationSettings.AvailableLocales.Locales;
		public void ChooseNextLanguage()
		{
			int index = Locale.IndexOf(LocalizationSettings.SelectedLocale);
			index++;
			if (index >= Locale.Count)
			{
				index = 0;
			}
			LocalizationSettings.SelectedLocale = Locale[index];
		}
	}
}