using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    private LineEngine _lineEngine;

    public void StartGame()
    {
        _lineEngine.Fire();
    }

    public void StopGame()
    {
        _lineEngine.Stop();
    }
}
