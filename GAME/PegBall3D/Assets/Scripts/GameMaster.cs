using System;
using System.Collections.Generic;
using UnityEngine;


public enum CameraPosition
{
    Desk,
    Game,
    Shelf,
    Upgrade,
    UpgradeMonitor
}

public enum ButtonDirection
{
    Left,
    Right
}
public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance { get; private set; }

    [SerializeField] public PegboardMaster PegboardMaster;

    [Header("Camera Positions")] 
    [SerializeField] private Transform _deskPosition;
    [SerializeField] private Transform _gamePosition;
    [SerializeField] private Transform _shelfPosition;
    [SerializeField] private Transform _upgradePosition;
    [SerializeField] private Transform _upgradeMonitorPosition;
    [Header("Other References")]
    [SerializeField] private Player player;

    [SerializeField] private UIManager _uiManager;

    private Dictionary<CameraPosition, Transform> cameraPositions;
    private List<CameraPosition> cyclePositions = new List<CameraPosition>
    {
        CameraPosition.Desk,
        CameraPosition.Shelf,
        CameraPosition.Upgrade
    };
    
    private int currentCycleIndex = 0;
    private CameraPosition currentPosition = CameraPosition.Desk;
    
    public bool IsPlayerFocused { get; private set; }
    public bool IsInGame
    {
        get => (currentPosition == CameraPosition.Game);
    }
    
    public CameraPosition CurrentPosition
    {
        get => currentPosition;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        cameraPositions = new Dictionary<CameraPosition, Transform>
        {
            { CameraPosition.Desk, _deskPosition },
            { CameraPosition.Game, _gamePosition },
            { CameraPosition.Shelf, _shelfPosition },
            { CameraPosition.Upgrade, _upgradePosition },
            { CameraPosition.UpgradeMonitor, _upgradeMonitorPosition }
        };
        
        MoveCameraTo(currentPosition);
        IsPlayerFocused = false;
        _uiManager.SetButtonState(!IsPlayerFocused);
    }

    private void HandleFocusSwitch()
    {
        var beforeSwitch = currentPosition;
        switch (currentPosition)
        {
            case CameraPosition.Desk:
                currentPosition = CameraPosition.Game;
                break;
            case CameraPosition.Game:
                currentPosition = CameraPosition.Desk;
                break;
            case CameraPosition.Shelf:
                break;
            case CameraPosition.Upgrade:
                currentPosition = CameraPosition.UpgradeMonitor;
                break;
            case CameraPosition.UpgradeMonitor:
                currentPosition = CameraPosition.Upgrade;
                break;
        }

        // if we have changed focus, move camera
        if (currentPosition != beforeSwitch)
        {
            MoveCameraTo(currentPosition);
        }
    }

    public void MoveCameraCycle(ButtonDirection direction)
    {
        if (!IsPlayerFocused)
        {
            if (direction == ButtonDirection.Left)
            {
                currentCycleIndex = (currentCycleIndex - 1 + cyclePositions.Count) % cyclePositions.Count;
            }
            else
            {
                currentCycleIndex = (currentCycleIndex + 1) % cyclePositions.Count;
            }
            MoveCameraTo(cyclePositions[currentCycleIndex]);
        }
    }

    public void MoveCameraTo(CameraPosition newPosition)
    {
        currentPosition = newPosition;

        if (cameraPositions.TryGetValue(newPosition, out var targetTransform))
        {
            player.MoveToPosition(targetTransform);
        }

        int foundIndex = cyclePositions.IndexOf(newPosition);
        if (foundIndex != -1)
        {
            currentCycleIndex = foundIndex;
        }
    }

    public void TrySetPlayerFocused(bool focus)
    {
        if (CurrentPosition != CameraPosition.Shelf)
        {
            if (focus != IsPlayerFocused)
            {
                IsPlayerFocused = focus;
                HandleFocusSwitch();
                _uiManager.SetButtonState(!IsPlayerFocused);
            }
        }
    }


    public void ResetPlayButton()
    {
        _uiManager.ResetPlayButton();
    }
}
