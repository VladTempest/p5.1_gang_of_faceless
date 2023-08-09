using System;
using System.Collections.Generic;

namespace SaveSystem.FileDataHandlers
{
	public interface IFileDataHandler
	{
		Dictionary<object, object> Load();
		void Save(object state);
		void DeleteSaveFile();
	}
}