using System;
using System.Collections.Generic;
using System.Linq;
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
    
    private static System.Random rng = new System.Random();
    
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
    [SerializeField] private GameObject moneyPeg;

    [Header("Ball")] 
    [SerializeField] private GameObject ballPrefab;

    [Header("References")] 
    [SerializeField] private RawImage bg;
    [SerializeField] private List<MultComponent> _multiplierList;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _ballsLeftText;
    [SerializeField] private TMP_Text _pegsLeftText;
    
    [SerializeField] private GameObject _pegboard;
    [SerializeField] private PegboardUIManager _pegboardUIManager;
    [SerializeField] private GameObject _launcherPos;
    [SerializeField] private GameObject dotPrefab;

    [Header("audio refs")] 
    [SerializeField] private AudioClip normalPegAudioClip;
    [SerializeField] private AudioClip moneyPegAudioClip;
    [SerializeField] private AudioClip pegDeleteAudioClip;
    [SerializeField] private AudioClip bucketAudioClip;

    
    // private float MultHudAnimTime = 5f;
    private float _dotSpacing = 0.15f;
    private float _ballShootForce = 7f;
    private float _ballRadius;
    private int _currentLevelIndex = 0;
    private int _levelDifficulty = 1;
    private float _maxDots = 20;
    private float _maxDistance = 4f;
    // private bool _canShoot;
    private Vector2 direction;
    private int _score;

    private int currentMult = 0;

    private readonly int[] multValueArray = new[]
    {
        1,
        2,
        3,
        4,
        5,
        6,
        8,
        10
    };
    
    private int thisShotScore;

    private const int RedPegAmount = 18;
    private int redPegsLeft;

    private const int StartingBalls = 15;
    private int ballsLeft;
    
    private PegboardState _pegboardState = PegboardState.LoadingLevel;
    private LayerMask aimDotCollisionMask;

    private GameObject currentLevel;
    private List<GameObject> dots = new();
    private List<BasePeg> _pegList = new();
    private Queue<BasePeg> _hitPegList = new();
    private GameObject _ball;
    private bool doesBallExist = false;
    private Rigidbody2D _ballRB;

    private float _pegStartDespawnTimer = 0f;
    private const float PegStartDespawnTime = .85f;
    private float _pegDespawnTimer = 0f;
    private const float PegDespawnTime = .08f;

    private AudioSource _sfxAudioSource;
    private AudioSource _bucketAudioSource;

    private BasePeg thisPeg;
    public int RequiredScore
    {
        get => (450 * (int)Math.Pow(_levelDifficulty, 2));
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

        _sfxAudioSource = gameObject.AddComponent<AudioSource>();
        _bucketAudioSource = gameObject.AddComponent<AudioSource>();

    }

    void Start()
    {
        // LoadNextLevel();
        SetPegboardState(PegboardState.LoadingLevel);
        SetScore(0);
        
        UpdatePegboardHUD();
    }
    
    public void RegisterHitPeg(BasePeg peg)
    {
        if (_pegList.Contains(peg))
        {
            _pegList.Remove(peg);
            _hitPegList.Enqueue(peg);

            _sfxAudioSource.pitch = 1f * (Mathf.Pow(2,_hitPegList.Count / 12f) * UnityEngine.Random.Range(0.995f, 1.005f));

            AudioClip pegSound;
            
            switch (peg.PegType)
            {
                case PegType.NormalPeg:
                    pegSound = normalPegAudioClip;
                    break;
                case PegType.ScorePeg:
                    pegSound = normalPegAudioClip;
                    redPegsLeft--;
                    break;
                case PegType.MoneyPeg:
                    pegSound = moneyPegAudioClip;
                    break;
                default:
                    pegSound = normalPegAudioClip;
                    break;
            }
            _sfxAudioSource.PlayOneShot(pegSound, 0.2f);

            thisShotScore += peg.PegData.pegValue;
            Debug.Log(peg.PegData.pegValue);
            
            _pegStartDespawnTimer = 0;

        }
    }


    private void FixedUpdate()
    {
        UpdatePegboardHUD();
    }

    void Update()
    {
        // CheckHitPegs();

        switch (_pegboardState)
        {
            case PegboardState.ReadyToShoot:
                UpdateAimDots();
                
                break;
            case PegboardState.LoadingLevel:
                break;
            case PegboardState.WaitingForBall:

                if (doesBallExist && _ball.transform.position.y < -3.1)
                {
                    Destroy(_ball);
                    doesBallExist = false;
                    // once ball is gone score that ball's points
                    AddScore(thisShotScore * currentMult);
                    GameMaster.Instance.AddTextToPipe($"SCORED {thisShotScore * currentMult} POINTS!");
                }

                if (!doesBallExist && _hitPegList.Count == 0)
                {
                    if (ballsLeft > 0)
                    {
                        SetPegboardState(PegboardState.ReadyToShoot);
                    }
                    else
                    {
                        ScoreLevel();
                    }
                }
                
                
                break;
            case PegboardState.ResultsScreen:
                break;
            case PegboardState.LoseScreen:
                break;
        }

        bool ShouldUpdateStartDespawnTimer = false;
        
        if (!doesBallExist && _hitPegList.Count != 0)
        {
            ShouldUpdateStartDespawnTimer = true;
        }
        else if (doesBallExist && _ballRB.linearVelocity.magnitude < 0.05f)
        {
            ShouldUpdateStartDespawnTimer = true;
        }
        
        if (ShouldUpdateStartDespawnTimer)
            _pegStartDespawnTimer += Time.deltaTime;



        if (_pegStartDespawnTimer >= PegStartDespawnTime && _hitPegList.Count > 0)
        {
            _pegDespawnTimer += Time.deltaTime;

            if (_pegDespawnTimer >= PegDespawnTime)
            {
                _sfxAudioSource.pitch = 1f / (Mathf.Pow(2f,_hitPegList.Count*3f / 12f));
                _sfxAudioSource.PlayOneShot(pegDeleteAudioClip, 0.6f);
                    
                _pegDespawnTimer = 0f;
                thisPeg = _hitPegList.Peek();
                thisPeg.gameObject.SetActive(false);
                Destroy(thisPeg.gameObject);
                _hitPegList.Dequeue();

            }
        }
    }
    public void StartGame()
    {
        SetPegboardState(PegboardState.ReadyToShoot);
    }

    public void BallInBucket()
    {
        _bucketAudioSource.PlayOneShot(bucketAudioClip);
        ballsLeft++;
        GameMaster.Instance.AddTextToPipe("bucket shot!");
    }

    public void SetPegboardState(PegboardState newState)
    {
        _pegboardState = newState;
        bool shouldPegboardBeActive = false;

        switch (_pegboardState)
        {
            case PegboardState.ReadyToShoot:
                shouldPegboardBeActive = true;
                _pegboardUIManager.Show(PegboardScreen.Pegboard);
                thisShotScore = 0;
                break;
            case PegboardState.WaitingForBall:
                shouldPegboardBeActive = true;
                _pegboardUIManager.Show(PegboardScreen.Pegboard);
                
                break;
            case PegboardState.LoadingLevel:
                _pegboardUIManager.Show(PegboardScreen.Loading);
                LoadNextLevel();
                GameMaster.Instance.ResetPlayButton(); // turns play button back on
                // _canShoot = false;
                break;
            case PegboardState.ResultsScreen:
                _pegboardUIManager.Show(PegboardScreen.Scores);
                // _canShoot = false;
                break;
            case PegboardState.LoseScreen:
                _pegboardUIManager.Show(PegboardScreen.Lose);
                // _canShoot = false;
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

        ballsLeft = StartingBalls + GameMaster.Instance.ExtraBalls;
        redPegsLeft = RedPegAmount;

        LoadLevel(_currentLevelIndex);
    }

    private void ScoreLevel()
    {
        if (redPegsLeft > 0)
        {
            if (GameMaster.Instance.BonusLives > 0)
            {
                GameMaster.Instance.BonusLives--;
                SetPegboardState(PegboardState.ResultsScreen);
            }
            else
            {
                SetPegboardState(PegboardState.LoseScreen);
            }
        }
        else
        {
            SetPegboardState(PegboardState.ResultsScreen);
        }
    }

    private void LoadLevel(int index)
    {
        if (currentLevel != null)
            Destroy(currentLevel);

        bg.texture = levelDataList[index].LevelBackground;
        SetScore(0);
        currentLevel = Instantiate(levelDataList[index].LevelGameObject, _pegboard.transform);

        
        _pegList.Clear();
        // randomises list
        var placeholderPegs = currentLevel.GetComponentsInChildren<PlaceholderPeg>().OrderBy(_ => rng.Next()).ToList();

        int i = 0;
        int moneyPegsAdded = 0;
        foreach (var placeholder in placeholderPegs)
        {
            GameObject pegToAdd;
            if (i < 25)
            {
                pegToAdd = scorePeg;
                i++;
            }
            else if (moneyPegsAdded < GameMaster.Instance.BonusMoneyPegs)
            {
                pegToAdd = moneyPeg;
                moneyPegsAdded++;
            }
            else
            {
                pegToAdd = normalPeg;
            }
            var peg = Instantiate(pegToAdd, placeholder.transform.position, placeholder.transform.rotation, _pegboard.transform);
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
        // UpdateMultHUD();
        _scoreText.text = _score.ToString("D6");
    }

    public void AddScore(int points)
    {
        _score += points;
        // UpdateMultHUD();
        _scoreText.text = _score.ToString("D6");
    }

    private void UpdatePegboardHUD()
    {
        int highestMult = 1;
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

        currentMult = multValueArray[highestMult];

        if (highestMult >= 0 && highestMult < _multiplierList.Count)
        {
            _multiplierList[highestMult].SetState(MultHUDState.MAIN);
        }
        
        

        _scoreText.text = _score.ToString("D6");
        _pegsLeftText.text = redPegsLeft.ToString("D2");
        _ballsLeftText.text = ballsLeft.ToString("D2");
    }
    
    public void TryShootBall()
    {
        if (_pegboardState != PegboardState.ReadyToShoot) return;

        if (ballsLeft > 0)
        {
            GameObject ball = Instantiate(ballPrefab, _launcherPos.transform.position, Quaternion.identity, transform);
            _ball = ball;
            doesBallExist = true;
            _ballRB = _ball.GetComponent<Rigidbody2D>();

            ball.GetComponent<Rigidbody2D>().AddForce(direction * _ballShootForce, ForceMode2D.Impulse);

            ballsLeft--;

            SetPegboardState(PegboardState.WaitingForBall);
        }
        else
        {
            ScoreLevel();
        }


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
