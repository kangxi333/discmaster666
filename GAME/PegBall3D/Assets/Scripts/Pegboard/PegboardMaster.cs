using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PegboardState
{
    Playing,
    Loading,
    WaitingForBall
}

public enum ScoreTiers
{
    TimesTwo,
    TimesThree,
    TimesFour,
    TimesFive,
    TimesSix,
    TimesEight,
    TimesTen
}

public class PegboardMaster : MonoBehaviour
{
    public static readonly Dictionary<ScoreTiers, int> ScoreThresholds = new()
    {
        { ScoreTiers.TimesTwo, 300},
        { ScoreTiers.TimesThree, 480},
        { ScoreTiers.TimesFour, 560},
        { ScoreTiers.TimesFive, 800},
        { ScoreTiers.TimesSix, 1000},
        { ScoreTiers.TimesEight, 1200},
        { ScoreTiers.TimesTen, 1400}
    };
    
    // list of all levels
    [SerializeField] private List<LevelData> levelDataList;

    [Header("Peg Types")] 
    [SerializeField] private GameObject normalPeg;
    [SerializeField] private GameObject scorePeg;
    [Header("Ball")] [SerializeField] private GameObject ballPrefab;

    [Header("References")] 
    [SerializeField] private RawImage bg;
    [SerializeField] private List<RectTransform> _multiplierList = new List<RectTransform>();
    
    private int _score;
    
    private GameObject currentLevel;

    private List<GameObject> _pegList = new List<GameObject>();
    private List<GameObject> _hitPegList = new List<GameObject>();
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevel(0);
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

        foreach (GameObject hitPeg in _hitPegList)
        {
            Destroy(hitPeg);
        }
        _hitPegList.Clear();
    }

    public void LoadNextLevel()
    {
        // gets next level and loads it.
        // these will loop (level1 -> level2 -> ... -> level10 -> level1)
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
    }


    public int GetScore() => _score;

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
    private void SetMultHUD(int score)
    {
        
    }
}
