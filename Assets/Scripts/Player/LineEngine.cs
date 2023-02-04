using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class LineEngine : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float _speed = 1.0f;
    [SerializeField]
    [Min(0)]
    private float _angularSpeed = 90.0f;

    private Vector3 _currentDirection;

    [SerializeField]
    private Transform _lineRootTransform;
    private CancellationTokenSource _tokenSrc;
    [SerializeField]
    private CollisionSender _collisionSender;

    // Start is called before the first frame update
    void Start()
    {
        _tokenSrc = new CancellationTokenSource();
        _currentDirection = new Vector3(0, -1, 0);
        _UpdateLineRoot().Forget();
        _collisionSender.OnCollideToDeath += () =>
        {
            _tokenSrc.Cancel();
            Debug.Log("Death!!");
        };
    }

    private async UniTaskVoid _UpdateLineRoot()
    {
        CancellationToken token = _tokenSrc.Token;
        while (!token.IsCancellationRequested)
        {
            _UpdateDirection();
            Vector3 rootPos = _lineRootTransform.position;
            _lineRootTransform.position = rootPos + _currentDirection * _speed;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private void _UpdateDirection()
    {
        float horizontalInput = SimpleInput.GetAxis("Horizontal");
        var abs = (float)Unity.Mathematics.math.smoothstep(0.3, 1, Mathf.Abs(horizontalInput));
        float deg = abs * _angularSpeed * Time.deltaTime;
        deg *= Mathf.Sign(horizontalInput);
        _currentDirection = Quaternion.AngleAxis(deg, Vector3.forward) * _currentDirection;
    }

    private void OnDestroy()
    {
        if(!_tokenSrc.IsCancellationRequested)
            _tokenSrc.Cancel();
        _tokenSrc.Dispose();
    }
}
