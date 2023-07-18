using System;

namespace SaveSystem
{
	public interface ISaveable
	{
		//void LoadData(object data);
		(Type, object) SaveData();
	}
}