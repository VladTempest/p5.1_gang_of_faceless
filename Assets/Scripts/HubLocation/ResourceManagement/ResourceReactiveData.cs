
using Editor.Scripts.HubLocation.ReactiveData;

public class ResourceReactiveData
{
	public ReactiveInt Amount;

	public ResourceReactiveData(int resourceAmount)
	{
		Amount = new ReactiveInt(resourceAmount);
	}
}