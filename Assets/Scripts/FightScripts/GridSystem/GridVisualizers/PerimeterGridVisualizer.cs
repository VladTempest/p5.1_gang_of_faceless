using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FightScripts.GridSystem
{
	[Serializable]
	public struct VisualParametersSetForAction
	{
		public GridVisualType type;
		public Color color;
		public float width;
	}
	
	
	class PerimeterGridVisualizer : MonoBehaviour,IGridVisualizer
	{
		[SerializeField]
		private GameObject _meshPrefab;
		[SerializeField]
		private Material _defaultMaterial;

		[SerializeField]
		private Vector3 _combinedMeshPosition =  new(0, 0.1f, 0);

		[FormerlySerializedAs("_parent")] [SerializeField] private GameObject _combinedMeshContainer;

		[SerializeField]
		private List<VisualParametersSetForAction> _visualParametersSetForActionList;


		private List<GameObject> _combinedMeshes = new ();
		private List<MeshFilter> _combinedMeshFilters = new ();
		private List<MeshRenderer> _combinedMeshRenderers = new ();
		private List<Outline> _outlines = new ();
		
		


		public void UpdateGridVisuals(Dictionary<GridVisualType, List<GridPosition>> gridVisualDict)
		{
			SetUpCombinedMesh(gridVisualDict);
		}

		public void HideGridVisuals()
		{
			if (_combinedMeshes == null || !_combinedMeshes.Any()) return;
			foreach (var combinedMesh in _combinedMeshes)
			{
				Destroy(combinedMesh);
			}
		}

		private void SetUpCombinedMesh(Dictionary<GridVisualType, List<GridPosition>> gridVisualDict)
		{
			List<KeyValuePair<GridVisualType, List<GridPosition>>> gridVisualDictList = new List<KeyValuePair<GridVisualType, List<GridPosition>>>(gridVisualDict);
			
			_combinedMeshes = new();
			_combinedMeshFilters = new();
			_combinedMeshRenderers = new();
			_outlines = new();
			
			for (int i = 0; i < gridVisualDictList.Count; i++)
			{
				_combinedMeshes.Add(Instantiate(_meshPrefab, _combinedMeshContainer.transform));
				_combinedMeshFilters.Add(_combinedMeshes[i].GetComponent<MeshFilter>());
				_combinedMeshRenderers.Add(_combinedMeshes[i].GetComponent<MeshRenderer>());
				_combinedMeshes[i].transform.position = _combinedMeshPosition;
				
				CombineMeshes(gridVisualDictList[i], _combinedMeshFilters[i]);
				_combinedMeshRenderers[i].material = _defaultMaterial;
				
				foreach (var outline in _outlines)
				{
					Destroy(outline);
				}
				
				StartCoroutine(AddOutlineInNextFrame(gridVisualDictList[i].Key, i));
			}

		}

		private IEnumerator AddOutlineInNextFrame(GridVisualType gridVisualType, int indexOfCombinedMesh)
		{
			yield return null;
			
			if (_combinedMeshes[indexOfCombinedMesh] == null) yield break;
			_combinedMeshes[indexOfCombinedMesh].SetActive(false);
			
			_outlines.Add(gridVisualType == GridVisualType.Red
				? _combinedMeshes[indexOfCombinedMesh].AddComponent<EarlyOutline>()
				: _combinedMeshes[indexOfCombinedMesh].AddComponent<Outline>());
			
			_outlines[indexOfCombinedMesh].OutlineColor = GetVisualParametersSetForAction(gridVisualType).color;
			_outlines[indexOfCombinedMesh].OutlineWidth = GetVisualParametersSetForAction(gridVisualType).width;
			_outlines[indexOfCombinedMesh].OutlineMode = Outline.Mode.OutlineVisible;
			
			
			_combinedMeshes[indexOfCombinedMesh].SetActive(true);
		}
		
		private VisualParametersSetForAction GetVisualParametersSetForAction(GridVisualType gridVisualType)
		{
			foreach (var visualParametersSetForAction in _visualParametersSetForActionList)
			{
				if (visualParametersSetForAction.type == gridVisualType)
				{
					return visualParametersSetForAction;
				}
			}
			throw new Exception("VisualParametersSetForAction not found");
		}

		private void CombineMeshes(KeyValuePair<GridVisualType, List<GridPosition>> typeGridPositionsPair, MeshFilter combinedMeshFilter)
		{
			List<Vector3> vertices = typeGridPositionsPair.Value.ConvertAll(gridPosition => LevelGrid.Instance.GetWorldPosition(gridPosition));
			
			MeshFilter[] meshFilters = new MeshFilter[typeGridPositionsPair.Value.Count];
			for (int i = 0; i < vertices.Count; i++)
			{
				GameObject meshToCombine = Instantiate(_meshPrefab, vertices[i], new Quaternion(_meshPrefab.transform.rotation.x, _meshPrefab.transform.rotation.y, _meshPrefab.transform.rotation.z, _meshPrefab.transform.rotation.w), combinedMeshFilter.transform);
				meshFilters[i] = meshToCombine.GetComponent<MeshFilter>();
			}

			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			for (int i = 0; i < meshFilters.Length; i++)
			{
				combine[i].mesh = meshFilters[i].sharedMesh;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			}

			combinedMeshFilter.mesh = new Mesh();
			combinedMeshFilter.mesh.CombineMeshes(combine);

			for (int i = 0; i < typeGridPositionsPair.Value.Count; i++)
			{
				Destroy(meshFilters[i].gameObject);
			}
		}
	}
}