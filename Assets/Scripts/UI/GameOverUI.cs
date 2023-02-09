using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text historyScoreText;

    [SerializeField] private Button _showBtn;
    [SerializeField] private Button _hideBtn;
    [SerializeField] private GameObject _mainUI;

    private const string CURRENT_SCORE_TEXT = "Score: ";
    private const string HISTORY_HIGHEST_SCORE_TEXT = "Highest Score: ";

    public void Awake()
    {
        _showBtn.onClick.RemoveAllListeners();
        _showBtn.onClick.AddListener(_Show);
        _hideBtn.onClick.RemoveAllListeners();
        _hideBtn.onClick.AddListener(_Hide);
    }

    public void SetCurrentScore(int score)
    {
        currentScoreText.text = CURRENT_SCORE_TEXT + score.ToString();
    }

    public void SetHistoryScore(int historyScore)
    {
        historyScoreText.text = HISTORY_HIGHEST_SCORE_TEXT + historyScore.ToString();
    }

    private void _Show()
    {
        _mainUI.SetActive(true);
    }

    private void _Hide()
    {
        _mainUI.SetActive(false);
    }
}
