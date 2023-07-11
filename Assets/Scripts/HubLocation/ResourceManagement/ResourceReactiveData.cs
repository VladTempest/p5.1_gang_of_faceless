
using Editor.Scripts.HubLocation.ReactiveData;
using SaveSystem;

public class ResourceReactiveData : IDataPersistence
{
	public ReactiveInt Amount;

	public ResourceReactiveData(int resourceAmount, DataPersistenceManager dataPersistenceManager)
	{
		dataPersistenceManager.RegisterDataPersistence(this);
		Amount = new ReactiveInt(resourceAmount);
	}

	public void LoadData(GameData data)
	{
		Amount.Value = data.goldCount;
	}

	public void SaveData(ref GameData data)
	{
		data.goldCount = Amount.Value;
	}
}