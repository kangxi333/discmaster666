using System.Collections.Generic;
using UnityEngine;

public class PegboardMaster : MonoBehaviour
{
    // list of all levels
    [SerializeField] private GameObject[] levelPrefabs;
    
    private GameObject currentLevel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
