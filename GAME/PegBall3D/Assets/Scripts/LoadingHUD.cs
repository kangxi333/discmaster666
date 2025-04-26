using TMPro;
using UnityEngine;

public class LoadingHUD : MonoBehaviour
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
}
