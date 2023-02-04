using System.Threading;

using UnityEngine;

public class GameplayPresenter : MonoBehaviour
{
    [SerializeField]
    private CollisionSender _collisionSender;
    [SerializeField]
    private GameplayView _view;
    [SerializeField]
    private LineRootEngine _lineRootEngine;
    [SerializeField]
    private LineParameters _parameters;
    [SerializeField]
    private LineRendererManager _lineRendererManager;
    [SerializeField]
    private Transform _lineRoot;
    [SerializeField]
    private LineColliderGenerator _colliderGenerator;
    [SerializeField]
    private RebornHelper _rebornHelper;

    private LineDrawerManager _drawerManager;
    private CancellationTokenSource _cancellationTokenSource;
    private GameplayState _gameplayState = GameplayState.Start;

    private void Awake()
    {
        _drawerManager = new LineDrawerManager(_parameters);
        _colliderGenerator.Initialize(_drawerManager);
        _rebornHelper.Initialize(new RandomPickOnLatestLine(_lineRendererManager), () => _gameplayState == GameplayState.Reborn);
        _rebornHelper.OnRebornDestinationMade += _StartNewSession;
    }

    private void _StartGameplaySession(Vector3 startPosition, Vector3 startDirection)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogError("Should stop gameplay session first.");
            return;
        }
        _cancellationTokenSource = new CancellationTokenSource();
        LineRenderer lineRenderer = _lineRendererManager.GetNewRenderer();
        LineDrawer drawer = _drawerManager.GetLineDrawer(lineRenderer);
        _lineRoot.position = startPosition;
        drawer.Draw(_cancellationTokenSource.Token, _lineRoot).Forget();
        _lineRootEngine.Fire(startPosition, startDirection);
    }

    private void _ResumeSession(LineRenderer lineRenderer, Vector3 startDirection)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogError("Should stop gameplay session first.");
            return;
        }
        _cancellationTokenSource = new CancellationTokenSource();
        LineDrawer drawer = _drawerManager.GetLineDrawer(lineRenderer);
        Vector3 startPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        _lineRoot.position = startPosition;
        drawer.Draw(_cancellationTokenSource.Token, _lineRoot).Forget();
        _lineRootEngine.Fire(startPosition, startDirection); 
    }

    private void _StopGameplaySession()
    {
        _lineRootEngine.Stop();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void OnDestroy()
    {
        _StopGameplaySession();
    }

    public void Start()
    {
        _collisionSender.OnCollideToSomething += () =>
        {
            Debug.Log("Collide!!");
            _StopGameplaySession();
            _gameplayState = GameplayState.Reborn;
        };
        _StartGameplaySession(new Vector3(0, 0, 0), new Vector3(0, -1, 0));
        _gameplayState = GameplayState.PlayerSession;
    }

    private void _StartNewSession((Vector3 newDir, LineNode node)param)
    {
        (Vector3 dir, LineNode node) = param;
        if(node.Index == node.LineRenderer.positionCount - 1)
            _ResumeSession(node.LineRenderer, dir);
        else
            _StartGameplaySession(node.Position, dir);
        _gameplayState = GameplayState.PlayerSession; 
    }

    private void Update()
    {
        _view.Render(_gameplayState);
    }
}
