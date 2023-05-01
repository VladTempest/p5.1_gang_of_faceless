using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

namespace Editor.Scripts.HubLocation
{
	public class PlaceHolder : MonoBehaviour
	{
		[SerializeField] private GameObject _placeHolderCube;
		
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				_placeHolderCube.SetActive(false);
			}
		}
	}
}