using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresHUD : PegboardHUDBase
{

    [SerializeField] private List<Sprite> _backgroundsList = new List<Sprite>();
    [SerializeField] private Image _backgroundImage;

    private System.Random rand = new System.Random();    

    public override void UpdateHUD()
    {
        // randomises background
        _backgroundImage.sprite = _backgroundsList[rand.Next(_backgroundsList.Count)];
        
        // add scores here
    }
}
