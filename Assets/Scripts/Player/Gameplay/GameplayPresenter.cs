using System.Collections.Generic;
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
    [SerializeField]
    private RandomPickFromAabb _fromAabb;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField] private float _maxLife = 1000;
    [SerializeField] private float _life;
    [SerializeField] private float downLifeSpeed = 50f;

    [SerializeField] private int _initHardness = 0;
    [SerializeField] private int _hardness;

    private List<Effect> _effects = new List<Effect>();

    [SerializeField] private int _maxDepth = 0;

    private int _effectScore = 0;

    private LineDrawerManager _drawerManager;
    private CancellationTokenSource _cancellationTokenSource;
    private GameplayState _gameplayState = GameplayState.Start;

    private void Awake()
    {
        _drawerManager = new LineDrawerManager(_parameters);
        _colliderGenerator.Initialize(_drawerManager);
        // Pick from latest
        //_rebornHelper.Initialize(new RandomPickOnLatestLine(_lineRendererManager), () => _gameplayState == GameplayState.Reborn);
        // From aabb
        //_fromAabb.Initialize(_drawerManager, _lineRoot, _parameters);
        //_rebornHelper.Initialize(_fromAabb, () => _gameplayState == GameplayState.Reborn);
        // From whole
        _rebornHelper.Initialize(new RandomPickOnWhole(_lineRendererManager, _lineRoot, _parameters), () => _gameplayState == GameplayState.Reborn);
        _rebornHelper.OnRebornDestinationMade += _StartNewSession;

        _maxDepth = 0;
        _life = _maxLife;
        _hardness = _initHardness;
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
        _lineRootEngine.Fire(startPosition, startDirection, this);
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
        _lineRootEngine.Fire(startPosition, startDirection, this); 
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
        _collisionSender.OnCollideToSomething += _OnCollideToSomething;
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
    
    private void _UpdateLife()
    {
        if (_gameplayState != GameplayState.PlayerSession)
        {
            return;
        }

        _life = _life - _GetFinalDownLifeSpeed() * Time.deltaTime;
        var humidity = Mathf.Max(0, (int)((100f * _life) / _maxLife));
        _view.SetHumidity(humidity);
        
        if (humidity <= 0)
        {
            _gameplayState = GameplayState.Death;
            _StopGameplaySession();
            _view.SetScore(0 , 0);
        }
    }

    private void _OnCollideToSomething(Collider2D collider)
    {
        var sceneObject = collider.GetComponent<BaseSceneObject>();
        if (sceneObject != null)
        {
            sceneObject.PlayAudio(_audioSource);

            if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Self)
            {
                Debug.Log("Collide!!");
                _StopGameplaySession();
                _gameplayState = GameplayState.Reborn;
            }
            else if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Effect)
            {
                var effectSceneObject = sceneObject as EffectSceneObject;
                effectSceneObject.Apply(this);
                GameObject.Destroy(collider.gameObject);
            }
            else if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Obstacle)
            {
                var obstableSceneObject = sceneObject as ObstacleSceneObject;
                
                if (obstableSceneObject.Hardness > _hardness)
                {
                    Debug.Log("Collide!!");
                    _StopGameplaySession();
                    _gameplayState = GameplayState.Reborn;
                }
            }
        }
    }

    private void Update()
    {
        _UpdateLife();
        _UpdateEffects();
        _maxDepth = Mathf.Max(_maxDepth, _GetCurrentDepth());
        _view.Render(_gameplayState);
    }

    private void _UpdateEffects()
    {
        for (int i = _effects.Count - 1 ; i >= 0 ; i--)
        {
            var effect = _effects[i];
            effect.Duration -= Time.deltaTime;
            if (effect.Duration <= 0f)
            {
                _effects.RemoveAt(i);
            }
        }
    }

    private float _GetFinalDownLifeSpeed()
    {
        var finalDownLifeSpeed = downLifeSpeed;
        foreach (var effect in _effects)
        {
            if (effect.Type == Effect.EffectType.LifeDownChange)
            {
                finalDownLifeSpeed = finalDownLifeSpeed * (100 + effect.Value) / 100f;
            }
        }
        return finalDownLifeSpeed;
    }

    public float GetFinalSpeed(float speed)
    {
        var finalSpeed = speed;
        foreach (var effect in _effects)
        {
            if (effect.Type == Effect.EffectType.SpeedChange)
            {
                finalSpeed = finalSpeed * (100 + effect.Value) / 100f;
            }
        }
        return finalSpeed;
    }

    private int _GetFinalScore()
    {
        return _maxDepth * 10 + _effectScore;
    }

    private int _GetCurrentDepth()
    {
        return (int)Mathf.Abs(_lineRoot.position.y);
    }

    #region Effect
    public void AddLife(int delta)
    {
        _life = _life + delta;
    }

    public void AddEffectScore(int delta)
    {
        _effectScore = _effectScore + delta;
    }

    public void AddHardness(int delta)
    {
        _hardness = _hardness + delta;
    }

    public void AddEffect(Effect effect)
    {
        _effects.Add(effect);
    }
    #endregion
}
