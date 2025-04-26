using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresHUD : MonoBehaviour
{

    [SerializeField] private List<Sprite> _backgroundsList = new List<Sprite>();
    [SerializeField] private Image _backgroundImage;

    private Unity.Mathematics.Random rand = new Unity.Mathematics.Random();    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoresHUD()
    {
        // randomises background
        _backgroundImage.sprite = _backgroundsList[rand.NextInt(_backgroundsList.Count)];
    }
}
