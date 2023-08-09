using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Editor.Scripts.Localization
{
	public class LocaleSaveManager : MonoBehaviour, ISaveable
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			DataPersistenceManager.Instance.RegisterDataPersistence(this);
			RestoreData();
		}
		
		public (object, object) CaptureData()
		{
			return (GetType(), LocalizationSettings.AvailableLocales.Locales?.IndexOf(LocalizationSettings.SelectedLocale));
		}

		public void RestoreData()
		{
			var persistentDataValue = DataPersistenceManager.Instance.GetState(GetType());

			if (persistentDataValue == null) return;

			var locales = LocalizationSettings.AvailableLocales.Locales;
			LocalizationSettings.InitializationOperation.WaitForCompletion();
			
			if (locales == null) return;
			
			LocalizationSettings.SelectedLocale = locales[(int) persistentDataValue];
		}
	}
}