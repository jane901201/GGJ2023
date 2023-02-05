using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class LineDrawer
{
    private readonly LineParameters _lineParameters;
    private readonly LineRenderer _lineRenderer;
    public Bounds Aabb;
    
    private Transform _lineRoot;
    private Vector3 _previousPos;

    public LineDrawer(LineParameters parameters, LineRenderer lineRenderer)
    {
        _lineParameters = parameters;
        _lineRenderer = lineRenderer;
    }

    public async UniTaskVoid Draw(CancellationToken token, Transform lineRoot)
    {
        _lineRoot = lineRoot;
        _previousPos = _GetLinePos(_lineRoot.position);
        if (_lineRenderer.positionCount == 0)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _previousPos);
            Aabb = new Bounds
            {
                center = _previousPos,
                size = new Vector3(0, 0, 0.1f)
            };
        }
        else
            _lineRenderer.positionCount++;
        while (!token.IsCancellationRequested)
        {
            _Update();
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private static Vector3 _GetLinePos(Vector3 rootPos)
        => new Vector3(rootPos.x, rootPos.y, 0);

    private void _Update()
    {
        Vector3 rootPos = _lineRoot.position;
        Vector3 position = _GetLinePos(rootPos);
        _lineRenderer.startWidth = _lineRenderer.endWidth = _lineParameters.LineWidth;
        if (Vector3.Distance(_previousPos, position) > _lineParameters.LineInterval)
        {
            int previousCount = _lineRenderer.positionCount++;
            Vector3 previousPos = _lineRenderer.GetPosition(previousCount - 1); 
            Aabb.Encapsulate(previousPos);
            _previousPos = previousPos;
        }
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position);
    }
}
