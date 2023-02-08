using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
    private RandomPickFromAabb _fromAabb;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private LevelManager _levelManager;

    [SerializeField] private Animator _bloodAnimator;
    [SerializeField] private float _bloodThreshold = 30;

    [SerializeField] private SpriteRenderer _blackMask;

    [SerializeField] private float _maxLife = 1000;
    [SerializeField] private float _life;
    [SerializeField] private float downLifeSpeed = 50f;

    [SerializeField] private int _initHardness = 0;
    [SerializeField] private int _hardness;

    [SerializeField] private float waitAnimationSeconds = 4f;

    [SerializeField]
    private float _angularOffset;

    [SerializeField]
    private float _resetTime = 2f;
    private bool _isResetting;

    private List<Effect> _effects = new List<Effect>();

    [SerializeField] private int _maxDepth = 0;

    private int _effectScore = 0;

    private LineDrawerManager _drawerManager;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationTokenSource _tickTokenSource;
    private GameplayState _gameplayState = GameplayState.Start;

    [SerializeField]
    private Camera _cam;
    private float _camSize;

    [SerializeField]
    private float _zoomOutSpeed = 1.0f;

    private IRebornMechanism _rebornMechanism;

    [SerializeField]
    private float _rebornCountdownSeconds = 3f;
    [SerializeField]
    private Transform _headTransform;

    private void Awake()
    {
        _drawerManager = new LineDrawerManager(_parameters);
        _colliderGenerator.Initialize(_drawerManager);
        // Pick from latest
        //_rebornMechanism = new RandomPickOnLatestLine(_lineRendererManager);
        // From aabb
        //_fromAabb.Initialize(_drawerManager, _lineRoot, _parameters);
        // _rebornMechanism = _fromAabb;
        // From whole
        _rebornMechanism = new RandomPickOnWhole(_lineRendererManager, _lineRoot, _parameters, _angularOffset);

        _maxDepth = 0;
        _life = _maxLife;
        _hardness = _initHardness;
        _camSize = _cam.orthographicSize;

        var humidity = Mathf.Clamp((int)((100f * _life) / _maxLife), 0, 100);
        _view.SetHumidity(humidity);
    }

    private void _StartGameplaySession(Vector3 startPosition, Vector3 startDirection, bool first = true)
    {
        if (_cancellationTokenSource != null)
        {
            Debug.LogError("Should stop gameplay session first.");
            return;
        }
        _blackMask.gameObject.SetActive(true);
        _cam.orthographicSize = _camSize;
        Transform transform1 = _cam.transform;
        transform1.localPosition = new Vector3(0, 0, transform1.localPosition.z);
        _cancellationTokenSource = new CancellationTokenSource();
        LineRenderer lineRenderer = _lineRendererManager.GetNewRenderer();
        LineDrawer drawer = _drawerManager.GetLineDrawer(lineRenderer);
        _lineRoot.position = startPosition;
        drawer.Draw(_cancellationTokenSource.Token, _lineRoot).Forget();
        _lineRootEngine.Fire(startPosition, startDirection, this);
        if (first)
            return;
        _tickTokenSource?.Dispose();
        _tickTokenSource = new CancellationTokenSource();
        _TickReset(_tickTokenSource.Token).Forget();
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
        _tickTokenSource?.Dispose();
        _tickTokenSource = new CancellationTokenSource();
        _TickReset(_tickTokenSource.Token).Forget();
    }

    private void _StopGameplaySession()
    {
        _lineRootEngine.Stop();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _tickTokenSource?.Cancel();
        _tickTokenSource?.Dispose();
        _tickTokenSource = null;
    }

    private void OnDestroy()
    {
        _StopGameplaySession();
    }

    public void Start()
    {
        _collisionSender.OnCollideToSomething += _OnCollideToSomething;
        StartCoroutine(WaitForAnimationComplete());
    }

    private IEnumerator WaitForAnimationComplete()
    {
        yield return new WaitForSeconds(waitAnimationSeconds);
        while (!_levelManager.IsInit)
        {
            yield return null;
        }
        downLifeSpeed = _levelManager.DownLifeSpeed;

        _StartGameplaySession(new Vector3(0, 0, 0), new Vector3(0, -1, 0));
        _gameplayState = GameplayState.PlayerSession;
    }

    private void _StartNewSession((Vector3 newDir, LineNode node)param)
    {
        (Vector3 dir, LineNode node) = param;
        if(node.Index == node.LineRenderer.positionCount - 1)
            _ResumeSession(node.LineRenderer, dir);
        else
            _StartGameplaySession(node.Position, dir, false);
        _gameplayState = GameplayState.PlayerSession; 
    }

    private async UniTaskVoid _TickReset(CancellationToken token)
    {
        await UniTask.Delay((int)_resetTime * 1000, cancellationToken: token);
        _isResetting = false;
    }

    private void _UpdateLife()
    {
        if (_gameplayState != GameplayState.PlayerSession)
        {
            return;
        }

        _life = _life - _GetFinalDownLifeSpeed() * Time.deltaTime;
        var humidity = Mathf.Clamp((int)((100f * _life) / _maxLife), 0, 100);
        _view.SetHumidity(humidity);

        if (humidity <= _bloodThreshold)
        {
            _bloodAnimator.gameObject.SetActive(true);
            _bloodAnimator.speed = 1f + (1f-(humidity/_bloodThreshold)) * 4f;
        }
        else
        {
            _bloodAnimator.gameObject.SetActive(false);
        }
        
        if (humidity <= 0)
        {
            _gameplayState = GameplayState.BeforeDeath;
            _blackMask.gameObject.SetActive(false);
            _bloodAnimator.gameObject.SetActive(false);
            _StopGameplaySession();

            StartCoroutine(nameof(_TransitionToDeath));
        }
    }

    private IEnumerator _TransitionToDeath()
    {
        var bounds = _drawerManager.GetWholeBounds();
        var camTrans = _cam.transform;
        var position = camTrans.position;
        var newCamPos = new Vector3(bounds.center.x, bounds.center.y, position.z);
        var proj = _cam.projectionMatrix;
        var maxSizeScale = Mathf.Max(bounds.size.x * proj.m00, bounds.size.y * proj.m11) * 0.6f;

        var scaleTime = maxSizeScale / _zoomOutSpeed;
        float t = 0.0f;
        var distanceToCenter = Vector3.Distance(newCamPos, position);
        var direction = Vector3.Normalize(newCamPos - position);

        while (t < scaleTime)
        {
            _cam.orthographicSize = _camSize * Mathf.SmoothStep(1, maxSizeScale, t / scaleTime);
            var newPos = position + direction * Mathf.SmoothStep(1, distanceToCenter, t / scaleTime);
            camTrans.position = new Vector3(newPos.x, newPos.y, position.z);
            yield return null;
            t += Time.deltaTime;
        }

        _cam.orthographicSize = _camSize * maxSizeScale;
        camTrans.position = newCamPos;
        
        _gameplayState = GameplayState.Death;

        var historyScoreKey = string.Format("HISTORY_{0}", _levelManager.LevelId);
        int historyScore = PlayerPrefs.GetInt(historyScoreKey, 0);
        int currentScore = _GetFinalScore();
        historyScore = Mathf.Max(currentScore, historyScore);
        PlayerPrefs.SetInt(historyScoreKey, historyScore);

        _view.SetScore(currentScore, historyScore);

        yield break;
    }

    private void _OnCollideToSomething(Collider2D collider)
    {
        var sceneObject = collider.GetComponent<BaseSceneObject>();
        if (sceneObject != null)
        {
            sceneObject.PlayAudio(_audioSource);
            sceneObject.PlayAnimator();

            if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Self && !_isResetting)
            {
                Debug.Log("Collide!!");
                _StopGameplaySession();
                _Reborn();
            }
            else if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Effect)
            {
                var effectSceneObject = sceneObject as EffectSceneObject;
                effectSceneObject.Apply(this);
                GameObject.Destroy(collider.gameObject.GetComponent<Collider>());
                GameManager.Destroy(collider.gameObject, 0.5f);
            }
            else if (sceneObject.ObjectType == BaseSceneObject.SceneObjectType.Obstacle)
            {
                var obstableSceneObject = sceneObject as ObstacleSceneObject;
                
                if (obstableSceneObject.Hardness > _hardness)
                {
                    Debug.Log("Collide!!");
                    _life -= obstableSceneObject.DownLife;
                    if (_life <= 0)
                    {
                        _gameplayState = GameplayState.BeforeDeath;
                        _blackMask.gameObject.SetActive(false);
                        _bloodAnimator.gameObject.SetActive(false);
                        _StopGameplaySession();

                        StartCoroutine(nameof(_TransitionToDeath));
                    }
                    else
                    {
                        _StopGameplaySession();
                        _Reborn();
                    }
                }
            }
        }
    }

    private void _Reborn()
    {
        _DuplicateHead();
        
        _gameplayState = GameplayState.Reborn;
        var param = _rebornMechanism.GetDest(); 
        (Vector3 dir, LineNode node) = param;
        Vector3 newPos;
        Quaternion newDir;
        if (node.Index == node.LineRenderer.positionCount - 1)
            newPos = node.LineRenderer.GetPosition(node.LineRenderer.positionCount - 1);
        else
            newPos = node.Position;
        newDir = Quaternion.FromToRotation(Vector3.right, dir);

        _isResetting = true;

        _lineRoot.position = newPos;
        _headTransform.rotation = newDir;

        StartCoroutine(Countdown(param));
    }

    private void Update()
    {
        _UpdateLife();
        _UpdateEffects();
        _view.SetResistText(_hardness);
        _maxDepth = Mathf.Max(_maxDepth, _GetCurrentDepth());

        _blackMask.color = new Color(1f, 1f, 1f, _GetCurrentDepth() <= 50 ? 0f : Mathf.Min(1f, (_GetCurrentDepth() - 50f) / 50f));

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

    private IEnumerator Countdown((Vector3 newDir, LineNode node) param)
    {
        float currentSeconds = _rebornCountdownSeconds;
        while (currentSeconds > 0)
        {
            if (currentSeconds > 0)
            {
                _view.SetCountdown(currentSeconds);
                Debug.Log("Seconds " + currentSeconds);
                yield return new WaitForSeconds(1f);
                currentSeconds--;
            }
            if (currentSeconds == 0)
                _view.SetCountdown(currentSeconds);
        }
        
        _StartNewSession(param);
    }

    private void _DuplicateHead()
    {
        Transform transform1;
        Instantiate(_headTransform.gameObject, (transform1 = _headTransform.transform).position, transform1.rotation);
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
