using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresHUD : PegboardHUDBase
{

    [SerializeField] private List<Sprite> _backgroundsList = new List<Sprite>();
    [SerializeField] private Image _backgroundImage;

    private Unity.Mathematics.Random rand = new Unity.Mathematics.Random();    

    public void UpdateHUD()
    {
        // randomises background
        _backgroundImage.sprite = _backgroundsList[rand.NextInt(_backgroundsList.Count)];
        
        // add scores here
    }
}
