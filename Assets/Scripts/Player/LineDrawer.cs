using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class LineDrawer
{
    private readonly LineParameters _lineParameters;
    private readonly LineRenderer _lineRenderer;
    
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
        int positionCount = ++_lineRenderer.positionCount;
        _lineRenderer.SetPosition(positionCount - 1, _previousPos);
        while (!token.IsCancellationRequested)
        {
            _Update();
            await UniTask.Yield(PlayerLoopTiming.Update);
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
            _lineRenderer.positionCount++;
            _previousPos = position;
        }
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1, position);
    }
}
