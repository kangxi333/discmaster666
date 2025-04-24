using System.Collections.Generic;
using UnityEngine;

public enum PegboardState
{
    Playing,
    Loading,
    WaitingForBall
}
public class PegboardMaster : MonoBehaviour
{
    // list of all levels
    [SerializeField] private GameObject[] levelPrefabs;

    [Header("Peg Types")] [SerializeField] private GameObject normalPeg;
    [SerializeField] private GameObject scorePeg;
    
    private GameObject currentLevel;

    private List<GameObject> _pegList = new List<GameObject>();
    private List<GameObject> _hitPegList = new List<GameObject>();

    
    public int TotalPoints { get; private set; }
    
    
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

        // foreach (GameObject hitPeg in _hitPegList)
        // {
        //     Destroy(hitPeg);
        // }
        // _hitPegList.Clear();
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

        currentLevel = Instantiate(levelPrefabs[index], transform);

        PlaceholderPeg[] placeholderPegs = currentLevel.GetComponentsInChildren<PlaceholderPeg>();
        
        foreach (PlaceholderPeg placeholderPeg in placeholderPegs)
        {
            Transform tr = placeholderPeg.gameObject.transform;
            
            _pegList.Add(Instantiate(normalPeg, tr.position,Quaternion.identity));
            Destroy(placeholderPeg.gameObject);
        }
        



    }
}
