using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class LineScaler : MonoBehaviour
{
    [SerializeField]
    private Transform _lineRootCircle;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private LineParameters _parameters;
    [SerializeField]
    private CircleCollider2D _circleCollider;

    private void Update()
    {
        _lineRootCircle.localScale = new Vector3(_parameters.CircleRadius, _parameters.CircleRadius, 1);
        _lineRenderer.startWidth = _lineRenderer.endWidth = _parameters.LineWidth;
        _circleCollider.radius = _parameters.CircleColliderRadius / 2;
    }
}
