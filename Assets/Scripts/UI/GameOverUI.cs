using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text historyScoreText;

    public void SetCurrentScore(int score)
    {
        currentScoreText.text = "分數：" + score.ToString();
    }

    public void SetHistoryScore(int historyScore)
    {
        historyScoreText.text = "歷史最高分數：" + historyScore.ToString();
    }
}
