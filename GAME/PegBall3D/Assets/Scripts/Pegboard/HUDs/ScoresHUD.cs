using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoresHUD : PegboardHUDBase
{

    [SerializeField] private List<Sprite> _backgroundsList = new List<Sprite>();
    [SerializeField] private Image _backgroundImage;

    [SerializeField] private TMP_Text _scoreText;

    private System.Random rand = new System.Random();

    public override void UpdateHUD()
    {
        // randomises background
        _backgroundImage.sprite = _backgroundsList[rand.Next(_backgroundsList.Count)];

        _scoreText.text = $"Score: {GameMaster.Instance.CurrentScore.ToString("D6")}";

        // add scores here
    }
}
