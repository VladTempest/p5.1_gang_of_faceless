using System;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.FightScripts.Pathfinding.PathVisualisation
{
	[RequireComponent(typeof(LineRenderer))]
	public class PathRenderer : MonoBehaviour
	{
		public static PathRenderer Instance { get; private set; }
		
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private float _heightOfLine;
		[SerializeField] float _segmentSize = 0.2f;
		
		private LineSmoother _lineSmoother = new LineSmoother();


		public void ShowPath()
		{
			if (CheckIfIncorrectTimeToShow()) return;
			GridPosition endGridPosition = (GridPosition) UnitActionSystem.Instance.GetSelectedPosition();
			List<GridPosition> path = global::Pathfinding.Instance.FindPath(
				UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition(), endGridPosition, out var _);
			if (_lineRenderer != null)
			{
				_lineRenderer.gameObject.SetActive(true);
				DrawPathLine(path);
			}
		}

		public void HidePath()
		{
			if (_lineRenderer != null)
			{
				_lineRenderer.gameObject.SetActive(false);
				_lineRenderer.positionCount = 0;
			}
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

		private static bool CheckIfIncorrectTimeToShow()
		{
			return SelectedPositionIsNullOrEmpty() || UnitActionSystem.Instance.GetSelectedAction() is not MoveAction || UnitActionSystem.Instance.IsBusy || UnitActionSystem.Instance.GetSelectedUnit() == null || !TurnSystem.Instance.IsPlayerTurn;
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
}