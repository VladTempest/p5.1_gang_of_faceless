using System;

namespace SaveSystem
{
	public interface ISaveable
	{
		(Type, object) CaptureData();
		void RestoreData();
	}
}