using System;
using System.Collections;
using System.Collections.Generic;
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
		[FormerlySerializedAs("combinedMesh")] [SerializeField] 
		private GameObject _combinedMesh;
		[SerializeField]
		private Vector3 _combinedMeshPosition =  new(0, 0.1f, 0);
		
		[SerializeField]
		private Color _outlineColor = Color.white;
		[SerializeField]
		private float _outlineWidth = 2f;
		
		[SerializeField]
		private UnitActionSystem _actionSystem;

		[SerializeField] private List<VisualParametersSetForAction> _visualParametersSetForActionList;
		
		private BaseAction _action;
		private List<GridPosition> _validGridPositions;
		private MeshFilter _combinedMeshFilter;
		private MeshRenderer _combinedMeshRenderer;

		private Outline _outline;


		public void VisualizeGrid()
		{
			SetUpCombinedMesh();
		}

		public void UpdateGridVisuals(Unit unit)
		{
			SetUpCombinedMesh();
		}

		public void HideGridVisuals()
		{
			_combinedMesh.SetActive(false);
		}
		
		private void Awake()
		{
			_combinedMeshFilter = _combinedMesh.GetComponent<MeshFilter>();
			_combinedMeshRenderer = _combinedMesh.GetComponent<MeshRenderer>();
			
			_combinedMesh.transform.position = _combinedMeshPosition;
		}
		
		public void SetUpCombinedMesh()
		{
			_action = _actionSystem.GetSelectedAction();
			_validGridPositions = _action.GetValidGridPositions();

			CombineMeshes(_validGridPositions, _combinedMeshFilter);
			_combinedMeshRenderer.material = _defaultMaterial;

			if (_outline != null)
			{
				Destroy(_outline);
			}
			StartCoroutine(AddOutlineInNextFrame());
			
		}

		private IEnumerator AddOutlineInNextFrame()
		{
			yield return null;
			if (_outline != null) yield break;
			
			_outline = _combinedMesh.AddComponent<Outline>();
			if (_outline == null)
			{
				Debug.LogError("Outline script not found");
			}
			else
			{
				_outline.OutlineColor = _outlineColor;
				_outline.OutlineWidth = _outlineWidth;
			}
			
			_combinedMesh.SetActive(true);
		}

		private void CombineMeshes(List<GridPosition> gridPositions, MeshFilter combinedMeshFilter)
		{
			List<Vector3> vertices = gridPositions.ConvertAll(gridPosition => LevelGrid.Instance.GetWorldPosition(gridPosition));
			
			MeshFilter[] meshFilters = new MeshFilter[gridPositions.Count];
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

			for (int i = 0; i < gridPositions.Count; i++)
			{
				Destroy(meshFilters[i].gameObject);
			}
		}
	}
}