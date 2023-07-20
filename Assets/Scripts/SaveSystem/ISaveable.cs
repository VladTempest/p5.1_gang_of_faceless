using System;

namespace SaveSystem
{
	public interface ISaveable
	{
		(Type, object) SaveData();
		void LoadData();
	}
}