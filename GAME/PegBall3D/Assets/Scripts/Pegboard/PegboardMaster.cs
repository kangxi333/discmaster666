using System;
using System.Collections;
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
    // Base scores required for a multiplier
    public static readonly int[] scoreThresholds =
    {
        200*1,
        200*2 + 200,
        (int)(200*3 + 200*1.4),
        (int)(200*4 + 200*1.8),
        (int)(200*5 + 200*2),
        (int)(200*6 + 200*2.4),
        (int)(200*8 + 200*3),
        (int)(200*10 + 200*4)
    };
    
    // list of all levels
    [SerializeField] private List<LevelData> levelDataList;

    [Header("Peg Types")] 
    [SerializeField] private GameObject normalPeg;
    [SerializeField] private GameObject scorePeg;
    [Header("Ball")] [SerializeField] private GameObject ballPrefab;

    [Header("References")] 
    [SerializeField] private RawImage bg;
    [SerializeField] private List<MultComponent> _multiplierList = new List<MultComponent>();
    [SerializeField] private List<LevelData> _levelDataList = new List<LevelData>();
    [SerializeField] private TMP_Text _scoreText;
    [FormerlySerializedAs("_Pegboard")] [SerializeField] private GameObject _pegboard;
    
    [SerializeField] private GameObject _launcherPos;
    [SerializeField] private GameObject dotPrefab;

    [Header("HUD ui elements")]
    [SerializeField] private GameObject _pegboardHUD;
    [SerializeField] private GameObject _scoresHUD;
    [SerializeField] private GameObject _loadingHUD;
    [SerializeField] private GameObject _loseHUD;

    private List<GameObject> hudsList = new List<GameObject>();

    private float MultHudAnimTime = 5f;
    
    private int _score;
    
    private GameObject currentLevel;
    // aiming tracer variables
    private float _dotSpacing = .15f;
    private float _maxDots = 20;
    private float _maxDistance = 4f;
    private LayerMask aimDotCollisionMask;
    private float _ballRadius;

    
    private List<GameObject> dots = new List<GameObject>();

    
    // Player variables
    private bool _canShoot;
    private float _ballShootForce = 7f;

    private int _currentLevelIndex = 0;
    private int _levelDifficulty = 0;

    private List<GameObject> _pegList = new List<GameObject>();
    private List<GameObject> _hitPegList = new List<GameObject>();

    private PegboardState _pegboardState = PegboardState.LoadingLevel;

    private Vector2 direction;

    private void Awake()
    {
        aimDotCollisionMask = LayerMask.GetMask("2D");
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // LoadLevel(0);
        
        LoadNextLevel();
        SetPegboardState(PegboardState.LoadingLevel);
        
        SetScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _pegList.Count; i++)
        {
            if (_pegList[i].GetComponent<BasePeg>().IsHit)
            {
                _hitPegList.Add(_pegList[i]);
                _pegList.RemoveAt(i);
            }
        }

        // foreach (GameObject hitPeg in _hitPegList)
        // {
        //     Destroy(hitPeg);
        // }
        if (_pegboardState == PegboardState.ReadyToShoot)
        {
            
        }
        
        _hitPegList.Clear();
        
        UpdateAimDots();
    }

    public void SetPegboardState(PegboardState state)
    {
        _pegboardState = state;
        
        _loadingHUD.SetActive(false);
        _loseHUD.SetActive(false);
        _pegboardHUD.SetActive(false);
        _scoresHUD.SetActive(false);
        
        switch (_pegboardState)
        {
            case PegboardState.ReadyToShoot:
                _canShoot = true;
                _pegboard.SetActive(true);
                _pegboardHUD.SetActive(true);
                break;
            case PegboardState.LoadingLevel:
                _canShoot = false;
                _pegboard.SetActive(false);
                _loadingHUD.SetActive(true);
                break;
            case PegboardState.WaitingForBall:
                _canShoot = false;
                _pegboard.SetActive(true);
                _pegboardHUD.SetActive(true);
                break;
            case PegboardState.ResultsScreen:
                _canShoot = false;
                _pegboard.SetActive(false);
                _scoresHUD.SetActive(true);
                break;
            case PegboardState.LoseScreen:
                _canShoot = false;
                _pegboard.SetActive(false);
                _loseHUD.SetActive(true);
                break;
        }
        
    }

    public void LoadNextLevel()
    {
        _currentLevelIndex = (_currentLevelIndex + 1) % levelDataList.Count;
        if (_currentLevelIndex == 1) // if it has reset level
        {
            RaiseDifficulty();
        }
        LoadLevel(_currentLevelIndex);
    }
    private void LoadLevel(int index)
    {
        if (currentLevel != null)
            Destroy(null);

        bg.texture = levelDataList[index].LevelBackground;

        currentLevel = Instantiate(levelDataList[index].LevelGameObject, transform);

        PlaceholderPeg[] placeholderPegs = currentLevel.GetComponentsInChildren<PlaceholderPeg>();
        
        
        // create array of ints from 0 - placeholderpegs.count
        // shuffle int arrays
        // use first 25 as indices for scoring pegs for 25 random pegs
        
        foreach (PlaceholderPeg placeholderPeg in placeholderPegs)
        {
            Transform tr = placeholderPeg.gameObject.transform;
            
            _pegList.Add(Instantiate(normalPeg, tr.position,Quaternion.identity));
            Destroy(placeholderPeg.gameObject);
        }
        
        _ballRadius = ballPrefab.transform.localScale.x * ballPrefab.GetComponent<CircleCollider2D>().radius;
    }
    public int GetScore() => _score;

    private void RaiseDifficulty()
    {
        _levelDifficulty += 1;
    }

    public void SetScore(int score)
    {
        _score = score;
        SetMultHUD(_score);
    }

    public void AddScore(int score)
    {
        _score += score;
        SetMultHUD(_score);
    }
    
    // TODO: SOMETHING IS WRONG HERE BUT I DONT KNOW WHAT YET (the 10x MULTIPLIER IS NEVER SET INACTIVE)
    private void SetMultHUD(int score)
    {
        int highestMult = -1;
        
        for (int i = 0; i < _multiplierList.Count -1; i++)
        {
            if (score >= scoreThresholds[i])
            {
                _multiplierList[i].SetState(MultHUDState.ACTIVE);
                highestMult = i;
            }
            else
            {
                _multiplierList[i].SetState(MultHUDState.INACTIVE);
            }
        }

        if (highestMult > -1)
        {
            _multiplierList[highestMult+1].SetState(MultHUDState.MAIN);
        }
    }

    public void TryShootBall()
    {
        GameObject ball = Instantiate(ballPrefab, _launcherPos.transform.position, Quaternion.identity, transform);
        ball.GetComponent<Rigidbody2D>().AddForce(direction * _ballShootForce, ForceMode2D.Impulse);
    }

    public void SetAimDirection(Vector2 dir)
    {
        direction = (dir - new Vector2(Screen.width / 2f, Screen.height)).normalized;
    }
    
    private void UpdateAimDots() // terribly unoptimised, will fix later™
    {
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();

        Vector2 origin = _launcherPos.transform.position;
        Vector2 velocity = (direction * _ballShootForce)/_dotSpacing; // ← start with initial "direction" * force
    
        float distanceTravelled = 0f;
        Vector2 currentPos = origin;
        float sizeMultiplier = 1f;

        for (int i = 0; i < _maxDots; i++)
        {
            // Simulate the next step with current velocity
            RaycastHit2D hit = Physics2D.CircleCast(currentPos, _ballRadius, velocity.normalized, _dotSpacing, aimDotCollisionMask);
            Vector2 nextPos;

            if (hit.collider != null)
            {
                nextPos = hit.point;
                SpawnDot(nextPos, sizeMultiplier);
                break; // stop at first hit
            }
            else
            {
                nextPos = currentPos + velocity.normalized * _dotSpacing;
                distanceTravelled += _dotSpacing;

                if (distanceTravelled > _maxDistance)
                    break;

                SpawnDot(nextPos, sizeMultiplier);
                currentPos = nextPos;
            }

            // Apply gravity to velocity (Y axis)
            velocity.y += Physics2D.gravity.y * _dotSpacing;

            sizeMultiplier *= 0.95f;
        }
    }

    private void SpawnDot(Vector2 position, float sizeMultiplier = 1f)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity, transform);
        dot.transform.localScale *= sizeMultiplier;
        dots.Add(dot);
    }
    
}
