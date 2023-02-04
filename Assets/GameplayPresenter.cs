using UnityEngine;

public class GameplayPresenter : MonoBehaviour
{
    [SerializeField]
    private CollisionSender _collisionSender;
    [SerializeField]
    private GameplayView _view;

    private int _x;
    
    public void Start()
    {
        _collisionSender.OnCollideToSomething += () =>
        {
            Debug.Log("Collide!!");
            _view.StopGameplaySession();
        };
        _view.Initialize();
        _view.StartGameplaySession(new Vector3(_x++, 0, 0), new Vector3(0, -1, 0));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            _view.StopGameplaySession();
            _view.StartGameplaySession(new Vector3(_x++, 0, 0), new Vector3(0, -1, 0));
        }
    }
}
