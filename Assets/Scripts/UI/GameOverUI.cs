using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text historyScoreText;

    public void SetCurrentScore(int score)
    {
        currentScoreText.text = "���ơG" + score.ToString();
    }

    public void SetHistoryScore(int historyScore)
    {
        historyScoreText.text = "���v�̰����ơG" + historyScore.ToString();
    }
}
