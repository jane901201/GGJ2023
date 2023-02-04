using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class LineRootEngine : MonoBehaviour
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

    public void Fire(Vector3 position, Vector3 direction)
    {
        if(_tokenSrc != null)
            return;
        _tokenSrc = new CancellationTokenSource();
        _lineRootTransform.position = position;
        _currentDirection = direction;
        _UpdateLineRoot().Forget();
    }

    public void Stop()
    {
        _tokenSrc?.Cancel();
        _tokenSrc?.Dispose();
        _tokenSrc = null;
    }

    private async UniTaskVoid _UpdateLineRoot()
    {
        CancellationToken token = _tokenSrc.Token;
        while (!token.IsCancellationRequested)
        {
            _UpdateDirection();
            Vector3 rootPos = _lineRootTransform.position;
            _lineRootTransform.position = rootPos + _currentDirection * _speed * Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private void _UpdateDirection()
    {
        float horizontalInput = SimpleInput.GetAxis("Horizontal") * -1;
        var abs = (float)Unity.Mathematics.math.smoothstep(0.3, 1, Mathf.Abs(horizontalInput));
        float deg = abs * _angularSpeed * Time.deltaTime;
        deg *= Mathf.Sign(horizontalInput);
        _currentDirection = Quaternion.AngleAxis(deg, Vector3.forward) * _currentDirection;
    }

    private void OnDestroy()
    { 
        _tokenSrc?.Cancel();
        _tokenSrc?.Dispose();
        _tokenSrc = null;
    }
}
