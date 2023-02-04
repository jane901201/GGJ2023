using System;
using System.Threading;

using UnityEngine;
using UnityEngine.Serialization;

public class GameplayView : MonoBehaviour
{
    [FormerlySerializedAs("_lineEngine")]
    [SerializeField]
    private LineRootEngine _lineRootEngine;
    [SerializeField]
    private LineParameters _parameters;
    [SerializeField]
    private LineRendererManager _lineRendererManager;
    [SerializeField]
    private Transform _lineRoot;

    private LineDrawerManager _drawerManager;
    private CancellationTokenSource _cancellationTokenSource;

    public void Initialize()
    {
        _drawerManager = new LineDrawerManager(_parameters);
    }

    public void StartGameplaySession(Vector3 startPosition, Vector3 startDirection)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogError("Should stop gameplay session first.");
            return;
        }
        _cancellationTokenSource = new CancellationTokenSource();
        LineRenderer lineRenderer = _lineRendererManager.GetLineRenderer();
        LineDrawer drawer = _drawerManager.GetLineDrawer(lineRenderer);
        _lineRoot.position = startPosition;
        drawer.Draw(_cancellationTokenSource.Token, _lineRoot).Forget();
        _lineRootEngine.Fire(startPosition, startDirection);
    }

    public void StopGameplaySession()
    {
        _lineRootEngine.Stop();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void OnDestroy()
    {
        StopGameplaySession();
    }
}
