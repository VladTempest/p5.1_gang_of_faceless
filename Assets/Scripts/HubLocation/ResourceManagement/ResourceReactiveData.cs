
using Editor.Scripts.HubLocation.ReactiveData;

public class ResourceReactiveData
{
	public ReactiveInt GoldCount;

	public ResourceReactiveData(int goldCount)
	{
		GoldCount = new ReactiveInt(goldCount);
	}
}