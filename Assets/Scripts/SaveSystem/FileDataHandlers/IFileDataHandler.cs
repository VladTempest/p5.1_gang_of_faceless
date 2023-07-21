using System;
using System.Collections.Generic;

namespace SaveSystem.FileDataHandlers
{
	public interface IFileDataHandler
	{
		Dictionary<Type, object> Load();
		void Save(object state);
		void DeleteSaveFile();
	}
}