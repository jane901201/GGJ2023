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
            _view.StopGameplaySession();
        };
        _view.Initialize();
        _view.StartGameplaySession(new Vector3(0, 0, 0));
    }
}
