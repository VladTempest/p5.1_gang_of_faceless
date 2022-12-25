using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isOpen;
    
    private GridPosition _gridPosition;
    private Animator _animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private Action _onInteractComplete;
    private float _timer = 0.5f;
    private bool _isActive;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this);

        if (_isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _isActive = false;
            _onInteractComplete();
        }
    }

    public void Interact(Action onInteractComplete)
    {
        _onInteractComplete = onInteractComplete;
        _isActive = true;
        _timer = 0.5f;
        if (_isOpen)
        {
            CloseDoor();
        }
        else
        {
           OpenDoor();
        }
    }

    public void OpenDoor()
    {
        _isOpen = true;
        _animator.SetBool(IsOpen, true);
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition,true);
    }

    public void CloseDoor()
    {
        _isOpen = false;
        _animator.SetBool(IsOpen, false);
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition,false);
    }
    
    
}
