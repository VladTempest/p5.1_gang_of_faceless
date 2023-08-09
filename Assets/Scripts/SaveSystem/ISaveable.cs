using System;

namespace SaveSystem
{
	public interface ISaveable
	{
		(object, object) CaptureData();
		void RestoreData();
	}
}