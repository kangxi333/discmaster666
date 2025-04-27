using TMPro;
using UnityEngine;

public class LoadingHUD : PegboardHUDBase
{
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text difficultyNumberText;
    [SerializeField] private TMP_Text requiredScoreText;

    public void SetLoadingText(string levelName, string difficultyNumber, string requiredScore)
    {
        levelNameText.text = levelName;
        difficultyNumberText.text = difficultyNumber;
        requiredScoreText.text = requiredScore;
    }

    public override void UpdateHUD()
    {
        SetLoadingText(
            GameMaster.Instance.PegboardMaster.CurrentLevelName,
            GameMaster.Instance.PegboardMaster.DifficultyNumber.ToString(),
            GameMaster.Instance.PegboardMaster.RequiredScore.ToString("D6")
            );
    }
}
