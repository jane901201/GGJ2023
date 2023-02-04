using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _template;

    private bool _taken;
    
    private void Awake()
    {
        _template.enabled = false;
        _template.positionCount = 0;
        _template.useWorldSpace = false;
    }

    public LineRenderer GetLineRenderer()
    {
        if (!_taken)
        {
            _taken = true;
            _template.enabled = true;
            return _template;
        }
        var newGo = new GameObject();
        newGo.transform.SetParent(transform);
        var lineRenderer = newGo.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
        return lineRenderer;
    }
}
