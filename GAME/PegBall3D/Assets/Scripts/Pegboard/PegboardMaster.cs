using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum PegboardState
{
    ReadyToShoot,
    LoadingLevel,
    WaitingForBall,
    ResultsScreen,
    LoseScreen
}

public enum AimNudge
{
    Left,
    Right
}

public class PegboardMaster : MonoBehaviour
{
    public static readonly int[] scoreThresholds =
    {
        200 * 1,
        200 * 3,
        (int)(200 * 4.4f),
        (int)(200 * 5.8f),
        (int)(200 * 7f),
        (int)(200 * 8.4f),
        (int)(200 * 11f),
        (int)(200 * 14f)
    };

    [SerializeField] private List<LevelData> levelDataList;

    [Header("Peg Types")] 
    [SerializeField] private GameObject normalPeg;
    [SerializeField] private GameObject scorePeg;

    [Header("Ball")] 
    [SerializeField] private GameObject ballPrefab;

    [Header("References")] 
    [SerializeField] private RawImage bg;
    [SerializeField] private List<MultComponent> _multiplierList;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _pegboard;
    [SerializeField] private PegboardUIManager _pegboardUIManager;
    [SerializeField] private GameObject _launcherPos;
    [SerializeField] private GameObject dotPrefab;

    private float MultHudAnimTime = 5f;
    private float _dotSpacing = 0.15f;
    private int _score;
    private float _ballShootForce = 7f;
    private float _ballRadius;
    private int _currentLevelIndex = -1;
    private int _levelDifficulty = 1;
    private float _maxDots = 20;
    private float _maxDistance = 4f;
    private bool _canShoot;
    private Vector2 direction;

    private PegboardState _pegboardState = PegboardState.LoadingLevel;
    private LayerMask aimDotCollisionMask;

    private GameObject currentLevel;
    private List<GameObject> dots = new();
    private List<BasePeg> _pegList = new();
    private List<GameObject> _ballsList = new();
    
    public int RequiredScore
    {
        get => (200 * (int)Math.Pow(_levelDifficulty, 2));
    }

    public string CurrentLevelName
    {
        get => levelDataList[_currentLevelIndex].LevelName;
    }

    public int DifficultyNumber
    {
        get => _levelDifficulty;
    }

    public int MoneyPegValue
    {
        get => (int)(_levelDifficulty * (RequiredScore / 10));
    }
    
    
    private void Awake()
    {
        aimDotCollisionMask = LayerMask.GetMask("2D");
    }

    void Start()
    {
        LoadNextLevel();
        SetPegboardState(PegboardState.LoadingLevel);
        SetScore(0);
    }
    
    public void RegisterHitPeg(BasePeg peg)
    {
        if (_pegList.Contains(peg))
        {
            _pegList.Remove(peg);
        }
    }


    void Update()
    {
        CheckHitPegs();

        switch (_pegboardState)
        {
            case PegboardState.ReadyToShoot:
                
                break;
            case PegboardState.LoadingLevel:
                break;
            case PegboardState.WaitingForBall:
                break;
            case PegboardState.ResultsScreen:
                break;
            case PegboardState.LoseScreen:
                break;
        }

        UpdateAimDots();
    }

    private void CheckHitPegs()
    {
        for (int i = _pegList.Count - 1; i >= 0; i--)
        {
            if (_pegList[i].IsHit)
            {
                Destroy(_pegList[i].gameObject);
                _pegList.RemoveAt(i);
            }
        }
    }

    public void StartGame()
    {
        SetPegboardState(PegboardState.ReadyToShoot);
        
    }

    public void SetPegboardState(PegboardState newState)
    {
        _pegboardState = newState;
        bool shouldPegboardBeActive = false;

        switch (_pegboardState)
        {
            case PegboardState.ReadyToShoot:
            case PegboardState.WaitingForBall:
                shouldPegboardBeActive = true;
                _pegboardUIManager.Show(PegboardScreen.Pegboard);
                _canShoot = true;
                break;
            case PegboardState.LoadingLevel:
                _pegboardUIManager.Show(PegboardScreen.Loading);
                LoadNextLevel();
                GameMaster.Instance.ResetPlayButton(); // turns play button back on
                _canShoot = false;
                break;
            case PegboardState.ResultsScreen:
                _pegboardUIManager.Show(PegboardScreen.Scores);
                _canShoot = false;
                break;
            case PegboardState.LoseScreen:
                _pegboardUIManager.Show(PegboardScreen.Lose);
                _canShoot = false;
                break;
        }

        _pegboard.SetActive(shouldPegboardBeActive);
    }

    public void LoadNextLevel()
    {
        _currentLevelIndex++;

        if (_currentLevelIndex >= levelDataList.Count)
        {
            _currentLevelIndex = 0;
            RaiseDifficulty();
        }

        LoadLevel(_currentLevelIndex);
    }

    private void LoadLevel(int index)
    {
        if (currentLevel != null)
            Destroy(currentLevel);

        bg.texture = levelDataList[index].LevelBackground;
        SetScore(0);
        currentLevel = Instantiate(levelDataList[index].LevelGameObject, _pegboard.transform);

        
        _pegList.Clear();
        foreach (var placeholder in currentLevel.GetComponentsInChildren<PlaceholderPeg>())
        {
            var peg = Instantiate(normalPeg, placeholder.transform.position, placeholder.transform.rotation, _pegboard.transform);
            _pegList.Add(peg.GetComponent<BasePeg>());
            Destroy(placeholder.gameObject);
        }


        _ballRadius = ballPrefab.transform.localScale.x * ballPrefab.GetComponent<CircleCollider2D>().radius;
        
        
    }

    private void RaiseDifficulty()
    {
        _levelDifficulty++;
    }

    public void SetScore(int score)
    {
        _score = score;
        UpdateMultHUD();
        _scoreText.text = _score.ToString("D6");
    }

    public void AddScore(int points)
    {
        _score += points;
        UpdateMultHUD();
        _scoreText.text = _score.ToString("D6");
    }

    private void UpdateMultHUD()
    {
        int highestMult = -1;
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (_score >= scoreThresholds[i])
            {
                _multiplierList[i].SetState(MultHUDState.ACTIVE);
                highestMult = i;
            }
            else
            {
                _multiplierList[i].SetState(MultHUDState.INACTIVE);
            }
        }

        if (highestMult >= 0 && highestMult < _multiplierList.Count)
        {
            _multiplierList[highestMult].SetState(MultHUDState.MAIN);
        }
    }
    
    public void StartPegboardGame()
    {
        
    }

    public void TryShootBall()
    {
        if (!_canShoot) return;

        GameObject ball = Instantiate(ballPrefab, _launcherPos.transform.position, Quaternion.identity, transform);
        _ballsList.Add(ball);

        ball.GetComponent<Rigidbody2D>().AddForce(direction * _ballShootForce, ForceMode2D.Impulse);

        _canShoot = false;
        SetPegboardState(PegboardState.WaitingForBall);
    }

    public void SetAimDirection(Vector2 screenPosition)
    {
        direction = (screenPosition - new Vector2(Screen.width / 2f, Screen.height)).normalized;
    }

    private void UpdateAimDots()
    {
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();

        Vector2 origin = _launcherPos.transform.position;
        Vector2 velocity = (direction * _ballShootForce) / _dotSpacing;
        Vector2 currentPos = origin;
        float distanceTravelled = 0f;
        float sizeMultiplier = 1f;

        for (int i = 0; i < _maxDots; i++)
        {
            var hit = Physics2D.CircleCast(currentPos, _ballRadius, velocity.normalized, _dotSpacing, aimDotCollisionMask);

            Vector2 nextPos;
            if (hit.collider != null)
            {
                nextPos = hit.point;
                SpawnTracerDot(nextPos, sizeMultiplier);
                break;
            }
            else
            {
                nextPos = currentPos + velocity.normalized * _dotSpacing;
                distanceTravelled += _dotSpacing;

                if (distanceTravelled > _maxDistance)
                    break;

                SpawnTracerDot(nextPos, sizeMultiplier);
                currentPos = nextPos;
            }

            velocity.y += Physics2D.gravity.y * _dotSpacing;
            sizeMultiplier *= 0.95f;
        }
    }

    private void SpawnTracerDot(Vector2 position, float sizeMultiplier = 1f)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity, _launcherPos.transform);
        dot.transform.localScale *= sizeMultiplier;
        dots.Add(dot);
    }
}
