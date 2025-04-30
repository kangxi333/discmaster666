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
    private enum Song
    {
        Song2,
        Game,
        Console
    }
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

    [Header("songs")]
    [SerializeField] private AudioClip song2;
    [SerializeField] private AudioClip consoleMusic;
    [SerializeField] private AudioClip gameMusic;

    private AudioSource _musicPlayer;
    private Song currentSong;

    private Dictionary<CameraPosition, Transform> cameraPositions;
    private List<CameraPosition> cyclePositions = new List<CameraPosition>
    {
        CameraPosition.Desk,
        CameraPosition.Shelf,
        CameraPosition.Upgrade
    };
    
    private int currentCycleIndex = 0;
    private CameraPosition currentPosition = CameraPosition.Desk;

    public float ScoreMultiplier = 1f;
    public float ScoreMultiplierMultiplier = 1f;
    public float BallSizeMultiplier = 1f;
    public float RequiredScoreMultiplier = 1f;
    public int ExtraBalls = 1;
    public int BonusMoneyPegs = 0;
    public int BonusLives = 0;
    
    public int CurrentScore { get; private set; }
    
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
            // return;
        }
        Instance = this;

        _musicPlayer = GetComponent<AudioSource>();
        
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

    private void Start()
    {
        SetSong(Song.Console);
        SetSong(Song.Song2);
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

    public void AddTextToPipe(string text)
    {
        _uiManager.scrollingPipeText.AddText(text);
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

        switch (currentPosition)
        {
            case CameraPosition.Desk:
                SetSong(Song.Song2);
                break;
            case CameraPosition.Game:
                SetSong(Song.Game);
                break;
            case CameraPosition.Shelf:
                SetSong(Song.Song2);
                break;
            case CameraPosition.Upgrade:
                SetSong(Song.Song2);
                break;
            case CameraPosition.UpgradeMonitor:
                SetSong(Song.Console);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetSong(Song song)
    {
        if (song != currentSong)
        {
            currentSong = song;
            switch (currentSong)
            {
                case Song.Song2:
                    _musicPlayer.clip = song2;
                    break;
                case Song.Game:
                    _musicPlayer.clip = gameMusic;
                    break;
                case Song.Console:
                    _musicPlayer.clip = consoleMusic;
                    break;
                default:
                    _musicPlayer.clip = song2;
                    break;
            }
            _musicPlayer.Play();
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

    public void DestroyPlayerInput()
    {
        player._playerInput.Dispose();
    }

    public void AddScore(int scoreToAdd)
    {
        CurrentScore = scoreToAdd;
    }

    public void SetScore(int score)
    {
        CurrentScore = score;
    }
}
