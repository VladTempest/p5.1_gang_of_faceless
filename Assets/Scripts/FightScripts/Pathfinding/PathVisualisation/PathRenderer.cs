using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.Utils.PoolScripts;
using GridSystems;
using UnityEngine;

namespace Scripts.FightScripts.Pathfinding.PathVisualisation
{
	[RequireComponent(typeof(LineRenderer))]
	public class PathRenderer : MonoBehaviour
	{
		public static PathRenderer Instance { get; private set; }
		
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private float _heightOfLine;
		[SerializeField] private float _segmentSize = 0.2f;
		[SerializeField] private PathVisualisationType _pathType = PathVisualisationType.Line;

		private LineSmoother _lineSmoother = new LineSmoother();
		private List<GameObject> _selectedGridVisuals = new List<GameObject>();

		private PoolProvider _poolProvider;

		public void ShowPath()
		{
			if (CheckIfIncorrectTimeToShow()) return;
			GridPosition endGridPosition = (GridPosition) UnitActionSystem.Instance.GetSelectedPosition();
			List<GridPosition> path = global::Pathfinding.Instance.FindPath(
				UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition(), endGridPosition, out var _);

			switch (_pathType)
			{
				case PathVisualisationType.Line:
					ShowLinePath(path);
					break;
				case PathVisualisationType.Grids:
					ShowGridPath(path);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ShowLinePath(List<GridPosition> path)
		{
			if (_lineRenderer != null)
			{
				_lineRenderer.gameObject.SetActive(true);
				DrawPathLine(path);
			}
		}

		private void ShowGridPath(List<GridPosition> path)
		{
			if (path.Count < 2)
			{
				ConvenientLogger.LogError(nameof(PathRenderer), GlobalLogConstant.IsPathFindingLogEnabled, "Path is too short");
			}
			path.RemoveAt(path.Count - 1);
			
			List<Vector3> worldPositionsFromPath = path.ConvertAll(position => LevelGrid.Instance.GetWorldPosition(position));
			
			//Despawn visuals not presented in worldPositionsFromPath
			for (int i = _selectedGridVisuals.Count - 1; i >= 0; i--)
			{
				if (!worldPositionsFromPath.Contains(_selectedGridVisuals[i].transform.position))
				{
					_poolProvider.DespawnToPool(PoolsEnum.SelectedGridVisual, _selectedGridVisuals[i]);
					_selectedGridVisuals.RemoveAt(i);
				}
			}
			
			//Spawn new visuals for worldPositionsFromPath
			foreach (var worldPosition in worldPositionsFromPath)
			{
				if (!_selectedGridVisuals.Exists(visual => visual.transform.position == worldPosition))
				{
					_selectedGridVisuals.Add(_poolProvider.SpawnFromPool(PoolsEnum.SelectedGridVisual, worldPosition, Quaternion.identity));
				}
			}
		}
		
		public void HidePath()
		{
			switch (_pathType)
			{
				case PathVisualisationType.Line:
					HideLinePath();
					break;
				case PathVisualisationType.Grids:
					HideGridPath();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HideGridPath()
		{
			foreach (var selectedGridVisual in _selectedGridVisuals)
			{
				_poolProvider.DespawnToPool(PoolsEnum.SelectedGridVisual, selectedGridVisual);
			}
			_selectedGridVisuals.Clear();
		}

		private void HideLinePath()
		{
			if (_lineRenderer != null)
			{
				_lineRenderer.gameObject.SetActive(false);
				_lineRenderer.positionCount = 0;
			}

			return;
		}

		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("There are many singletonss");
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}

		private void Start()
		{
			SetUpPathRenderer();
		}

		private void SetUpPathRenderer()
		{
			_poolProvider = PoolProvider.Instance;

			switch (_pathType)
			{
				case PathVisualisationType.Line:
					_lineRenderer.enabled = true;
					break;
				case PathVisualisationType.Grids:
					_lineRenderer.enabled = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			UnitActionSystem.Instance.OnSelectedPositionChanged += OnSelectedPositionChanged;
			UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
			UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged;
			TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
		}

		private void OnDestroy()
		{
			UnitActionSystem.Instance.OnSelectedPositionChanged -= OnSelectedPositionChanged;
			UnitActionSystem.Instance.OnBusyChanged -= OnBusyChanged;
			TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
			UnitActionSystem.Instance.OnSelectedUnitChanged -= OnSelectedUnitChanged;
		}

		private void OnSelectedUnitChanged(object sender, EventArgs e)
		{
			if (CheckIfIncorrectTimeToShow())
			{
				HidePath();
				return;
			}
			
			ShowPath();
		}

		private void OnTurnChanged(object sender, EventArgs e)
		{
			if (!TurnSystem.Instance.IsPlayerTurn)
			{
				HidePath();
			}
		}

		private void OnBusyChanged(object sender, bool isBusy)
		{
			if (isBusy)
			{
				HidePath();
			}
		}

		private void OnSelectedPositionChanged(object sender, OnSelectedPositionChangedArgs e)
		{
			if (SelectedPositionIsNullOrEmpty() || UnitActionSystem.Instance.GetSelectedAction() is not MoveAction || UnitActionSystem.Instance.IsBusy)
			{
				HidePath();
				return;
			}
			
			ShowPath();
		}

		private static bool CheckIfIncorrectTimeToShow()
		{
			return SelectedPositionIsNullOrEmpty() || UnitActionSystem.Instance.GetSelectedAction() is not MoveAction || UnitActionSystem.Instance.IsBusy || UnitActionSystem.Instance.GetSelectedUnit() == null || !TurnSystem.Instance.IsPlayerTurn;
		}

		private static bool SelectedPositionIsNullOrEmpty()
		{
			return UnitActionSystem.Instance.GetSelectedPosition() == null || UnitActionSystem.Instance.GetSelectedPosition() == new GridPosition(0,0);
		}

		private void DrawPathLine(List<GridPosition> path)
		{
			Vector3[] inputGridPositions = path.ConvertAll(LevelGrid.Instance.GetWorldPosition).ToArray();
			Vector3[] smoothedLine = _lineSmoother.SmoothLine(inputGridPositions, _segmentSize);
			
			_lineRenderer.positionCount = smoothedLine.Length;
			for (int i = 0; i < smoothedLine.Length; i++)
			{
				var lineNodeWorldPosition = new Vector3(smoothedLine[i].x, _heightOfLine, smoothedLine[i].z);
				_lineRenderer.SetPosition(i, lineNodeWorldPosition);
			}
		}
	}

	internal enum PathVisualisationType
	{
		Line,
		Grids
	}
}