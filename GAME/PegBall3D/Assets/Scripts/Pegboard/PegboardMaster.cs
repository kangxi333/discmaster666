using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PegboardState
{
    Playing,
    Loading,
    WaitingForBall
}public class PegboardMaster : MonoBehaviour
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

    private float MultHudAnimTime = 5f;
    
    private int _score;
    
    private GameObject currentLevel;

    private List<GameObject> _pegList = new List<GameObject>();
    private List<GameObject> _hitPegList = new List<GameObject>();
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevel(0);
        
        // TODO: REMOVE ME (TEST)
        SetScore(350);
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
    
}
