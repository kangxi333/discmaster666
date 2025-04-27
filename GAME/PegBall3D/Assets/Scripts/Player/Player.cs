using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Transform _lastPosition;
    private Transform _targetTransform;

    private PlayerInput _playerInput;

    private float _moveSpeed = 2f;
    private float _rotateSpeed = 5f;

    private float _timescaleMult = 4f;
    private float _timescaleSpeedMult = 8f; // terrible naming but how fast it goes to desired timescaleMult

    private bool _isMoving = false;

    private Vector2 lastMousePos;
    private Vector2 lastScroll;
    
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
            GameMaster.Instance.TrySetPlayerFocused(!GameMaster.Instance.IsPlayerFocused);
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

        

        if (GameMaster.Instance.IsPlayerFocused && GameMaster.Instance.IsInGame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            
            if (mousePos != lastMousePos)
            {
                GameMaster.Instance.PegboardMaster.SetAimDirection(mousePos);
            }
            
            if (_playerInput.Player.Fire.WasPressedThisFrame())
            {
                GameMaster.Instance.PegboardMaster.TryShootBall();
            }
            
            lastMousePos = mousePos; // always a frame behind
        }

        if (_playerInput.Player.SpeedUp.IsPressed()) // changes timescale on rmb
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, _timescaleMult, _timescaleSpeedMult * Time.deltaTime);
        }
        else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, _timescaleSpeedMult * Time.deltaTime);
        }
    }

    public void MoveToPosition(Transform position)
    {
        _lastPosition = _targetTransform;
        _targetTransform = position;

        _isMoving = true;

    }
}
