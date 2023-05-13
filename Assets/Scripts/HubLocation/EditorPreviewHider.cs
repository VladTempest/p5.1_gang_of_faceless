using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class EditorPreviewHider : MonoBehaviour
	{
		[SerializeField] private GameObject _placeHolderCube;

		private void Awake()
		{
			_placeHolderCube.gameObject.SetActive(false);
		}
	}
}