using System;
using SaveSystem;

namespace SoundSystemScripts
{
	public class AudioVolumeSaver : ISaveable
	{
		public (object, object) CaptureData()
		{
			//return (GetType(), new SaveData((_parameterToChange, _volumeSlider.value)));
			return (typeof(AudioVolumeSaver), null);
		}

		public void RestoreData()
		{
			var persistentDataDict = DataPersistenceManager.Instance.GetState(GetType());

			var persistentData = (SaveData) persistentDataDict;
			if (persistentData == null) return;

			//_volumeSlider.value = persistentData.VolumePair.Item2;
		}
        
		[Serializable]
		class SaveData
		{
			public (AudioMixerExposeParams,float) VolumePair;
		
			public SaveData((AudioMixerExposeParams,float) volumePair)
			{
				VolumePair = volumePair;
			}
		}
	}
}