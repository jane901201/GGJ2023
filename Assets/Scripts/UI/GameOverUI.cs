using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text historyScoreText;

    private const string CURRENT_SCORE_TEXT = "Score: ";
    private const string HISTORY_HIGHEST_SCORE_TEXT = "Highest Score: ";

    public void SetCurrentScore(int score)
    {
        currentScoreText.text = CURRENT_SCORE_TEXT + score.ToString();
    }

    public void SetHistoryScore(int historyScore)
    {
        historyScoreText.text = HISTORY_HIGHEST_SCORE_TEXT + historyScore.ToString();
    }
}
