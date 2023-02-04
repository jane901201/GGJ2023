using UnityEngine;

public class GameplayPresenter : MonoBehaviour
{
    [SerializeField]
    private CollisionSender _collisionSender;
    [SerializeField]
    private GameplayView _view;
    
    public void Start()
    {
        _collisionSender.OnCollideToSomething += () =>
        {
            Debug.Log("Collide!!");
            _view.StopGame();
        };
        _view.StartGame();
    }
}
