using System;
using Editor.Scripts.HubLocation.ReactiveData;
using SaveSystem;

[Serializable]
public class ResourceReactiveData
{
	public ReactiveInt Amount;

	public ResourceReactiveData(int resourceAmount)
	{
		Amount = new ReactiveInt(resourceAmount);
	}
}