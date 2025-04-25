using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform _lastPosition;
    private Transform _targetTransform;

    private PlayerInput _playerInput;

    private float _moveSpeed = 2f;
    private float _rotateSpeed = 5f;

    private bool _isMoving = false;
    
    public bool IsFocused { get; private set; }

    private void Awake()
    {
        _playerInput = new PlayerInput(); // initialises new input system
        _playerInput.Enable();

        _targetTransform = transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var input = _playerInput.Player;

        if (input.Tab.WasPressedThisFrame())
        {
            IsFocused = !IsFocused;
            GameMaster.Instance.SetPlayerFocused(IsFocused);
        }

        if (_isMoving && _targetTransform != null)
        {
            transform.position =
                Vector3.Lerp(transform.position, _targetTransform.position, Time.deltaTime * _moveSpeed);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, _targetTransform.rotation, Time.deltaTime * _rotateSpeed);
        }

        if (Vector3.Distance(transform.position, _targetTransform.position) < .01f &&
            Quaternion.Angle(transform.rotation, _targetTransform.rotation) < 0.05f)
        {
            _isMoving = false;
        }
        
    }

    public void MoveToPosition(Transform position)
    {
        _lastPosition = _targetTransform;
        _targetTransform = position;

        _isMoving = true;

    }
}
